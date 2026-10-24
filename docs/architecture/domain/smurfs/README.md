# Smurfs: first assisted roundtrip

This exercise covers the complete `Temple.Domain.Entities.Smurfs` namespace.

## Design and implementation

1. [Baseline](baseline.puml) records the original `Smurf` class.
2. [Proposal](proposed.puml) adds `SmurfRole` and the `Smurf.Role` property.
3. The forward step is a reviewed implementation patch, preserving existing
   constructor and `ToString()` behavior. It is not whole-file code generation.
4. [Implemented model](smurfs.puml) is extracted back from the C# declarations.
5. The [rendered diagram](smurfs.svg) shows the implemented namespace and relation.

The agreed behavior is:

- Roles have stable persisted values: `Unspecified = 0`, `Builder = 1`, `Farmer = 2`.
- A new Smurf defaults to `Unspecified`. `Name` still starts empty, and `ToString()`
  still returns the name, regardless of role.
- The new migration adds a non-null role column with database default `0`, so
  existing rows and inserts which omit the column receive `Unspecified`.
- Existing migrations are preserved. The migration is tested against isolated
  in-memory SQLite databases; it has not been applied to your working database.
- DTOs, application queries, mappings, and UI remain unchanged. Exposing or editing
  the role is deferred; this exercise changes Domain and persistence only.

Tests cover the constructor/display contract, upgrading existing data, roundtripping
every role, database defaults, downgrade preservation of IDs/names, and EF model
compatibility with the latest migration.

## Regenerate and reconcile

With Python 3.10+ and Graphviz's `dot` on PATH, run from the repository root:

```powershell
python docs/architecture/domain/smurfs/generate.py
python docs/architecture/domain/smurfs/generate.py --check
```

The first command regenerates `smurfs.puml` and `smurfs.svg` from C#. It never
changes the baseline, proposal, or C# source. The second is read-only: it compares
the current C# structure with both the proposal and the implemented diagram,
prints a diff, and exits unsuccessfully when they disagree.

For the next forward step, edit the proposal and describe the intended behavior
and persistence/UI scope. Implement the agreed change, regenerate the implemented
model, review any differences, and run the relevant tests. For a code-first change,
regenerate the implemented model and review its differences before updating the
proposal to reflect an accepted design.

The extractor supports this exercise's simple declarations, rather than arbitrary
C#. It rejects additional source files until its coverage is extended. Structural
agreement does not prove behavioral agreement: defaults, invariants, migrations,
and method bodies still require tests and review. Enum changes involving numeric
values, renames, or removals require an explicit persistence compatibility decision.
No new packages or runtime modeling framework are introduced.
