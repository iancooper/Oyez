# Modules, not layers — Intent

## Problem

Oyez's design (ADR 0001) treats assemblies as layers: `Oyez.Core` holds the domain and application, `Oyez.Adapters` the I/O, `Oyez.Cli` the commands. That is not what an assembly is for. An assembly is a unit of deployment and compilation, not a unit of design.

Two things follow:

1. **`Oyez.Core` is several modules in one assembly.** Its folders (`Announcements`, `Channels`, `Counting`, `Costing`, `Publishing`, `Previewing`, `Validating`, `Configuration`) are modules in everything but enforcement. Because they share an assembly, anything `internal` is visible to all of them, so nothing stops `Publishing` reaching into the internals of `Counting`. Their boundaries are a convention.
2. **The layering is implied by project references, and is the only rule.** Nothing says which folders may depend on which.

## Goal

Modules that are **deep and narrow**:

- **Narrow:** a small public surface (few public types and interfaces).
- **Deep:** it hides substantial behaviour, and its internals can change freely.

And a design where:

- **Modules are the unit of design.** Each module's public boundary is the only way in. Where a module is a folder in `Oyez.Core`, we decide whether to keep it there or to split it out, and how its boundary is made real.
- **Layers are enforced by architecture tests**, not by convention or by project references alone. The tests state which modules may depend on which, and fail the build when a dependency breaks the rule.
- **Tests sit only at a module's public boundary.** No `InternalsVisibleTo`. A test never reaches an internal type.
- **Test doubles stand in for a module only when another module is under test.** Mock (or fake) one module from another module's tests, never a class within the module under test. I/O ports, the clock and randomness remain legitimate doubles.

## To find out

- Which modules does `Oyez.Core` really contain, and what is each one's boundary today? Candidates: `Announcements`, `Channels`, `Counting`, `Costing`, `Publishing`, `Previewing`, `Validating`, `Configuration`.
- What does each module expose to the others that it need not, and what does it hide that others reach for?
- How do we make a boundary real? Options: separate assemblies, so `internal` is enforced by the compiler; or one assembly with architecture tests over namespaces. We need to weigh both.
- Which architecture-test tool, if any: for example NetArchTest or ArchUnitNET. Or hand-written reflection tests, to keep dependencies down.
- How does "tests only at the public boundary" apply today? ADR 0001 and `CLAUDE.md` test `Oyez.Core` through the Brighter commands and Darker queries. That is the application module's boundary, but behind it several modules are exercised only indirectly. Do the inner modules need their own boundary tests?
- What changes in `CLAUDE.md`, ADR 0001 and `.agent_instructions/` (the "narrow and deep" and "visibility" rules refer to assemblies).

## Not decided

Nothing here is a design. The spec needs requirements, a design and tasks before any code moves. Phase 3 (adapters) continues under spec 0001 meanwhile.
