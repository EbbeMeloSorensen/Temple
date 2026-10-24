"""Reverse this small Smurfs model into a diagram and compare it with its proposal.

Uses Python's standard library and Graphviz. The extractor supports the simple
declarations used by this exercise; it is not an arbitrary C# parser or code writer.
"""

from difflib import unified_diff
from html import escape
from pathlib import Path
import re
import subprocess
import sys
import xml.etree.ElementTree as ET

OUTPUT = Path(__file__).resolve().parent
SOURCE = OUTPUT.parents[3] / "Temple.Domain/Entities/Smurfs"


def model_from_code():
    if {path.name for path in SOURCE.glob("*.cs")} != {"Smurf.cs", "SmurfRole.cs"}:
        raise ValueError("The namespace contents changed; extend this focused extractor before regenerating.")
    smurf = (SOURCE / "Smurf.cs").read_text(encoding="utf-8-sig")
    role = (SOURCE / "SmurfRole.cs").read_text(encoding="utf-8-sig")
    namespace = re.search(r"namespace\s+([\w.]+)", smurf).group(1)
    if re.search(r"namespace\s+([\w.]+)", role).group(1) != namespace:
        raise ValueError("Smurf and SmurfRole must belong to the same namespace.")
    class_name = re.search(r"public class (\w+)", smurf).group(1)
    enum_name = re.search(r"public enum (\w+)", role).group(1)
    properties = re.findall(r"public ([\w?]+) (\w+)\s*\{\s*get;\s*set;\s*\}", smurf)
    members = [f"+{name} : {type_name}" for type_name, name in properties]
    if re.search(rf"public {class_name}\(\)", smurf):
        members.append(f"+{class_name}()")
    for return_type, name, arguments in re.findall(r"public (?:override )?(\w+) (\w+)\(([^)]*)\)", smurf):
        members.append(f"+{name}({arguments}) : {return_type}")
    enum_members = [f"{name} = {value}" for name, value in re.findall(r"(\w+)\s*=\s*(-?\d+)", role)]
    lines = ["@startuml", "skinparam packageStyle folder", "skinparam classAttributeIconSize 0",
             f'package "{namespace}" {{', f"    class {class_name} {{"]
    lines += ["        " + member for member in members]
    lines += ["    }", f"    enum {enum_name} {{"]
    lines += ["        " + member for member in enum_members]
    lines.append("    }")
    associations = []
    for type_name, name in properties:
        if type_name.rstrip("?") == enum_name:
            cardinality = "0..1" if type_name.endswith("?") else "1"
            associations.append((class_name, enum_name, name, cardinality))
            lines.append(f'    {class_name} --> "{cardinality}" {enum_name} : {name}')
    lines += ["}", "@enduml"]
    return "\n".join(lines) + "\n", namespace, class_name, members, enum_name, enum_members, associations


def render_svg(namespace, class_name, members, enum_name, enum_members, associations):
    def node(name, rows, enumeration=False):
        title = ("&lt;&lt;enumeration&gt;&gt;<BR/>" if enumeration else "") + f"<B>{escape(name)}</B>"
        body = "".join(f'<TR><TD ALIGN="LEFT">{escape(row)}</TD></TR>' for row in rows)
        return (f'{name} [label=<<TABLE BORDER="1" CELLBORDER="0" CELLSPACING="0" CELLPADDING="8" '
                f'COLOR="#71839a"><TR><TD BGCOLOR="#e9f0f8">{title}</TD></TR><HR/>{body}</TABLE>>];')

    edges = [f'{source} -> {target} [arrowhead=vee,label="{role} [{cardinality}]",color="#256a91",fontname="Segoe UI",fontsize=12];'
             for source, target, role, cardinality in associations]
    dot = '\n'.join([
        'digraph G { graph [rankdir=LR,bgcolor="white",pad=0.4,nodesep=0.5,ranksep=0.9];',
        'node [shape=plaintext,fontname="Segoe UI",fontsize=13];',
        f'subgraph cluster_smurfs {{ label="{namespace}"; labelloc=t; labeljust=l; fontname="Segoe UI"; margin=28; color="#71839a";',
        node(class_name, members), node(enum_name, enum_members, True),
        *edges,
        '}}'
    ])
    result = subprocess.run(["dot", "-Tsvg"], input=dot, text=True, capture_output=True, check=True)
    tree = ET.ElementTree(ET.fromstring(result.stdout))
    svg = "{http://www.w3.org/2000/svg}"
    group = next(g for g in tree.getroot().iter(svg + "g") if g.attrib.get("class") == "cluster")
    polygon = group.find(svg + "polygon")
    points = [tuple(map(float, p.split(","))) for p in polygon.attrib["points"].split()]
    left, top, right, bottom = min(p[0] for p in points), min(p[1] for p in points), max(p[0] for p in points), max(p[1] for p in points)
    width = min(len(namespace) * 7.5 + 25, right - left)
    group.remove(polygon)
    for text in list(group.findall(svg + "text")):
        group.remove(text)
    ET.SubElement(group, svg + "path", {"d": f"M{left},{top} H{left+width} V{top+26} H{right} V{bottom} H{left} Z M{left},{top+26} H{left+width}",
                                       "fill": "white", "stroke": "#71839a"})
    text = ET.SubElement(group, svg + "text", {"x": str(left + 10), "y": str(top + 18), "font-family": "Segoe UI", "font-size": "13"})
    text.text = namespace
    ET.register_namespace("", "http://www.w3.org/2000/svg")
    tree.write(OUTPUT / "smurfs.svg", encoding="utf-8", xml_declaration=True)


if __name__ == "__main__":
    implemented, *model = model_from_code()
    if "--check" in sys.argv:
        for filename in ["proposed.puml", "smurfs.puml"]:
            expected = (OUTPUT / filename).read_text(encoding="utf-8")
            if expected != implemented:
                print("".join(unified_diff(expected.splitlines(True), implemented.splitlines(True),
                                           fromfile=filename, tofile="current C# model")))
                sys.exit(1)
        print("Current C# structure matches the proposal and the implemented diagram.")
    else:
        (OUTPUT / "smurfs.puml").write_text(implemented, encoding="utf-8")
        render_svg(*model)
        print("Regenerated the implemented diagram; baseline and proposal were not modified.")
