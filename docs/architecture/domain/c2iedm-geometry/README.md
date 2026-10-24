# C2IEDM Geometry

The seven concrete `Surface` specializations and `OrbitAreaAlignmentCode` belong
to `Temple.Domain.Entities.C2IEDM.Geometry.Surfaces`. The abstract `Surface` and
the other geometry categories remain in the parent namespace.

- [Overview](overview.svg): every type, with inheritance, namespace packages,
  and the `VerticalDistance`, `LinePoint`, and coordinate-system associations.
- [Complete class diagram](complete.svg): inheritance and all navigation properties.
- [Surfaces detail](surfaces.svg): the specializations and their associations to the parent namespace.
- [PlantUML source](geometry.puml): the complete model, with individual navigation relations.

Italic class names are abstract. Hollow triangles point to base classes. Blue
arrows show navigation properties, labeled with their names and target
multiplicities. Parallel references to the same type share an arrow in the SVG
views; each labeled role represents a separate property. Foreign-key GUID
properties are omitted because the associations already describe those references.
Multiplicities reflect the C# declarations; the diagrams do not assert composition
or exclusive ownership.

The overview includes the optional `AbsolutePoint.VerticalDistance` association,
both optional vertical distances on `GeometricVolume`, the bidirectional
`Line`/`LinePoint` association, and `LinePoint.Point`. It also shows
`RelativePoint.CoordinateSystem` and the three point references on
`PointReference`: `OriginPoint`, `XVectorPoint`, and `YVectorPoint`.
Other associations appear in the complete view.

## Regeneration

From the repository root, with Python 3.10+ and Graphviz's `dot` on PATH:

```powershell
python docs/architecture/domain/c2iedm-geometry/generate.py
```

Commit the regenerated SVG and PlantUML files with relevant code changes. The
generator uses the Python standard library and the existing Graphviz installation;
no application dependencies are added. It extracts the simple class declarations
and auto-properties used in this folder, rather than attempting to parse arbitrary
C#. Update the extractor if this model introduces other declaration forms.

## Persistence compatibility

The namespace move changes CLR type names, not table names or relationships.
The current EF model snapshot records the new names; the historical migration and
its target model retain the original names. `AppDataSchemaCompatibilityTests`
compares the current relational model with the latest migration and checks for
pending model changes. No database migration is needed for this refactoring.
