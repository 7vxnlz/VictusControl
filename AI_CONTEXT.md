# VictusControl AI Context

## Project

VictusControl is a future open-source C#/.NET Windows utility for the HP Victus Gaming Laptop 16-s0035nt / 7Z5Z2EA.

## Product Goal

Build a lightweight, maintainable utility for controlling and monitoring supported HP Victus laptop behavior. The project should be original, modular, and safety-first.

## Planned Platform

- Windows desktop
- C#/.NET
- UI framework not finalized
- GitHub Desktop assumed for version control

## Architecture Direction

Use a layered design:

- app/UI layer
- application orchestration layer
- domain models
- hardware abstraction interfaces
- HP-specific hardware implementation
- infrastructure for configuration, logging, updates, and diagnostics
- tests for logic and safety behavior

Avoid global hardware state, monolithic startup files, copied vendor apps, and broad feature creep.

## Reference Repositories

Expected references:

- `seerge/g-helper`
- `ib-3/ghelper-omen`
- `theantipopau/omencore`
- `breadeding/OmenSuperHub`
- `MasonDye/OmenXHub`
- `affaan-m/ECC`

References are research material only. They should not be copied wholesale or included in default AI context. Track local reference paths, branches, and commit SHAs in `REFERENCE_SOURCES.md`.

## Out Of Scope For This Layer

- application code
- C# solution/project creation
- hardware command implementation
- UI design
- driver installation
- reference repository modification

## Current Status

Bootstrap phase. The repository is receiving permanent AI workflow, token strategy, and assistant guidance files before application development begins.

