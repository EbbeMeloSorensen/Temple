"""Generate the Geometry diagrams from its simple class/auto-property declarations.

Requires Python 3.10+ and Graphviz's dot executable. This is intentionally a
focused extractor for this model, rather than a general-purpose C# parser.
"""

from collections import defaultdict
from html import escape
from pathlib import Path
import re
import subprocess
import xml.etree.ElementTree as ET


OUTPUT = Path(__file__).resolve().parent
ROOT = OUTPUT.parents[3]
SOURCE = ROOT / "Temple.Domain/Entities/C2IEDM/Geometry"
NAMESPACE = "Temple.Domain.Entities.C2IEDM.Geometry"
SVG = "{http://www.w3.org/2000/svg}"


def read_model():
    classes, enums, inheritance, associations = {}, {}, [], []
    for path in sorted(SOURCE.rglob("*.cs")):
        text = path.read_text(encoding="utf-8-sig")
        namespace = re.search(r"namespace\s+([\w.]+)", text).group(1)
        for name, body in re.findall(r"public enum (\w+)\s*\{([^}]+)\}", text):
            enums[name] = (namespace, re.findall(r"\b\w+\b", body))
        declaration = re.search(r"public\s+(abstract\s+)?class\s+(\w+)(?:\s*:\s*(\w+))?", text)
        if declaration is None:
            raise ValueError(f"No supported class declaration in {path}")
        abstract, name, parent = declaration.groups()
        properties = re.findall(r"public\s+(?:virtual\s+)?([\w<>?]+)\s+(\w+)\s*\{\s*get;", text)
        classes[name] = (namespace, bool(abstract), properties)
        if parent:
            inheritance.append((parent, name))
    for name, (_, _, properties) in classes.items():
        for type_name, role in properties:
            collection = re.fullmatch(r"ICollection<(\w+)>\??", type_name)
            target = collection.group(1) if collection else type_name.rstrip("?")
            if target in classes:
                multiplicity = "0..*" if collection else "0..1" if type_name.endswith("?") else "1"
                associations.append((name, target, role, multiplicity))
            elif target not in {"Guid", "double", "int"} and target not in enums:
                raise ValueError(f"Unsupported property {name}.{role}: {type_name}")
    return classes, enums, inheritance, associations


def write_plantuml(classes, enums, inheritance, associations):
    lines = ["@startuml", "skinparam packageStyle folder", "skinparam classAttributeIconSize 0",
             "hide empty methods", f'package "{NAMESPACE}" {{']
    for namespace in [NAMESPACE, NAMESPACE + ".Surfaces"]:
        if namespace != NAMESPACE:
            lines.append('  package "Surfaces" {')
        for name, (ns, abstract, properties) in classes.items():
            if ns != namespace:
                continue
            lines.append("    " + ("abstract " if abstract else "") + f"class {name} {{")
            for type_name, role in properties:
                if type_name.rstrip("?") in classes or type_name.startswith("ICollection<"):
                    continue
                if type_name.startswith("Guid") and role not in {"Id", "ID"}:
                    continue
                lines.append(f"      +{role} : {type_name}")
            lines.append("    }")
        for name, (ns, values) in enums.items():
            if ns == namespace:
                lines.extend([f"    enum {name} {{", *["      " + value for value in values], "    }"])
        if namespace != NAMESPACE:
            lines.append("  }")
    lines.append("}")
    lines.extend(f"{parent} <|-- {child}" for parent, child in inheritance)
    lines.extend(f'{source} --> "{multiplicity}" {target} : {role}'
                 for source, target, role, multiplicity in associations)
    lines.append("OrbitArea ..> OrbitAreaAlignmentCode")
    lines.append("@enduml")
    (OUTPUT / "geometry.puml").write_text("\n".join(lines) + "\n", encoding="utf-8")


def class_node(name, abstract, properties, classes):
    title = f"<I>{name}</I>" if abstract else name
    rows = []
    for type_name, role in properties:
        if type_name.rstrip("?") in classes or type_name.startswith("ICollection<"):
            continue
        if type_name.startswith("Guid") and role not in {"Id", "ID"}:
            continue
        rows.append(f'<TR><TD ALIGN="LEFT">+ {escape(role)} : {escape(type_name)}</TD></TR>')
    contents = '<HR/>' + "".join(rows) if rows else ""
    return (f'{name} [label=<<TABLE BORDER="1" CELLBORDER="0" CELLSPACING="0" CELLPADDING="6" '
            f'COLOR="#71839a"><TR><TD BGCOLOR="#e9f0f8"><B>{title}</B></TD></TR>{contents}</TABLE>>];')


def write_svg(view, classes, enums, inheritance, associations):
    surface_types = {name for name, (ns, _, _) in classes.items() if ns.endswith(".Surfaces")}
    selected = set(classes) if view != "surfaces" else surface_types | {"Location", "Surface", "Point", "Line"}
    lines = ['digraph G {',
             'graph [rankdir=LR, bgcolor="white", pad=0.6, nodesep=0.55, ranksep=1.8, splines=spline];',
             'node [shape=plaintext, fontname="Segoe UI", fontsize=12];',
             'edge [color="#53657b", penwidth=1.2, arrowsize=0.8];',
             'subgraph cluster_geometry { color="#71839a"; margin=30; label=" \\n ";']
    for nested in [False, True]:
        if nested:
            lines.append('subgraph cluster_surfaces { color="#71839a"; margin=24; label=" \\n ";')
        for name in sorted(selected):
            _, abstract, properties = classes[name]
            if (name in surface_types) == nested:
                lines.append(class_node(name, abstract, properties, classes))
        if nested:
            for name, (_, values) in enums.items():
                value_rows = "".join(f'<TR><TD ALIGN="LEFT">{value}</TD></TR>' for value in values)
                lines.append(f'{name} [label=<<TABLE BORDER="1" CELLBORDER="0" CELLSPACING="0" CELLPADDING="6" '
                             f'COLOR="#71839a"><TR><TD BGCOLOR="#f5f0e6">&lt;&lt;enumeration&gt;&gt;<BR/>'
                             f'<B>{name}</B></TD></TR><HR/>{value_rows}</TABLE>>];')
            lines.append("}")
    for parent, child in inheritance:
        if parent in selected and child in selected:
            lines.append(f'{parent} -> {child} [dir=back,arrowtail=empty];')
    groups = defaultdict(list)
    for source, target, role, multiplicity in associations:
        if view == "overview" and not {source, target} & {
            "VerticalDistance", "LinePoint", "CoordinateSystem", "PointReference"
        }:
            continue
        if source in selected and target in selected and (source, target) != ("LinePoint", "Line"):
            groups[source, target].append(f"{role} [{multiplicity}]")
    for (source, target), roles in groups.items():
        extra = ""
        if (source, target) == ("Line", "LinePoint"):
            roles, extra = ["Line [1] / LinePoints [0..*]"], "dir=both,arrowtail=vee,"
        label = "\\n".join(roles)
        lines.append(f'{source} -> {target} [constraint=false,{extra}arrowhead=vee,color="#256a91",'
                     f'fontcolor="#205674",fontname="Segoe UI",fontsize=11,label="{label}"];')
    lines.extend(['OrbitArea -> OrbitAreaAlignmentCode [style=dashed,arrowhead=open,label="uses",'
                  'fontname="Segoe UI",fontsize=11];', "}", "}"])
    result = subprocess.run(["dot", "-Tsvg"], input="\n".join(lines), text=True, capture_output=True, check=True)
    tree = ET.ElementTree(ET.fromstring(result.stdout))
    # Graphviz clusters provide layout; add UML package tabs inside their reserved header space.
    for group in tree.getroot().iter(SVG + "g"):
        if group.attrib.get("class") != "cluster":
            continue
        title = group.find(SVG + "title").text
        label = NAMESPACE if title == "cluster_geometry" else "Surfaces"
        width = 410 if title == "cluster_geometry" else 95
        coordinates = [tuple(map(float, point.split(","))) for point in group.find(SVG + "polygon").attrib["points"].split()]
        x, y = min(p[0] for p in coordinates), min(p[1] for p in coordinates)
        for text in list(group.findall(SVG + "text")):
            group.remove(text)
        ET.SubElement(group, SVG + "path", {"d": f"M{x},{y} v-28 h{width} v28", "fill": "white",
                                           "stroke": "#71839a", "stroke-width": "1.4"})
        text = ET.SubElement(group, SVG + "text", {"x": str(x + 12), "y": str(y - 9),
                                                  "font-family": "Segoe UI", "font-size": "13", "fill": "#24364b"})
        text.text = label
    ET.register_namespace("", "http://www.w3.org/2000/svg")
    tree.write(OUTPUT / f"{view}.svg", encoding="utf-8", xml_declaration=True)


if __name__ == "__main__":
    model = read_model()
    write_plantuml(*model)
    for view in ["overview", "complete", "surfaces"]:
        write_svg(view, *model)
    print(f"Generated {len(model[0])} classes, {len(model[1])} enum, and {len(model[3])} navigation references.")
