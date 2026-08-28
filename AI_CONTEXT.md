# VictusControl AI Context

## Project

VictusControl is an open-source C#/.NET Windows utility for the HP Victus Gaming Laptop 16-s0035nt / 7Z5Z2EA.

## Product Goal

Build a lightweight, maintainable utility for controlling and monitoring supported HP Victus laptop behavior. The project should be original, modular, and safety-first.

## Platform

- Windows desktop
- C#/.NET 8
- WPF app shell
- xUnit tests
- GitHub Desktop assumed for version control

## Architecture Direction

Use a layered design:

- `VictusControl.App`: Windows desktop entry project
- `VictusControl.Application`: orchestration and use-case boundaries
- `VictusControl.Domain`: pure Windows-independent domain models
- `VictusControl.Hardware.Abstractions`: read-only and later hardware-facing interfaces
- `VictusControl.Infrastructure`: configuration, logging, updates, and diagnostics infrastructure when introduced
- tests for domain, application, and safety behavior

Avoid global hardware state, monolithic startup files, copied vendor apps, broad feature creep, and write-capable hardware paths before capability discovery is proven safe.

## Reference Repositories

Expected references:

- `seerge/g-helper`
- `ib-3/ghelper-omen`
- `theantipopau/omencore`
- `breadeding/OmenSuperHub`
- `MasonDye/OmenXHub`
- `affaan-m/ECC`

References are research material only. They should not be copied wholesale or included in default AI context. Track local reference paths, branches, and commit SHAs in `REFERENCE_SOURCES.md`.

## Out Of Scope Until Explicitly Requested

- HP WMI command implementation
- fan control implementation
- telemetry polling implementation
- EC access
- BIOS setting writes
- driver installation
- vendor DLLs or proprietary HP binaries
- reference repository modification

## Current Status

`v0.3.0-windows-device-identity-provider` is complete. The solution skeleton exists, read-only device identity and capability contracts are defined, and Infrastructure includes a narrow Windows identity provider using standard read-only WMI queries. No HP WMI control methods, fan control, telemetry loops, EC access, BIOS writes, or write-capable hardware logic exists yet.
