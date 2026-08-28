# Next Milestone Plan

This plan defines the next safe milestone after `v0.2.0-device-identity-and-capability-contracts`. It is a planning document only and does not add implementation code, HP WMI calls, fan control, telemetry loops, EC access, BIOS writes, UI features, or hardware write logic.

## Recommended Milestone

Milestone: `v0.3.0-windows-device-identity-provider`

Create the first concrete read-only discovery adapter in `VictusControl.Infrastructure`: a Windows device identity provider that implements `IDeviceIdentityProvider` by reading standard Windows identity data only.

This milestone should identify the current machine well enough for later capability decisions, but it must not probe HP-specific WMI control methods or assume that any hardware capability is supported.

## Why This Comes Next

`v0.2.0` defined the domain contracts and read-only provider boundaries. The safest next step is to prove one narrow concrete adapter can feed those contracts from the host operating system.

This should come before settings/logging because the project needs real identity data to shape meaningful capability profiles and diagnostics. It should also come before a static capability loader because static profiles are risky without first knowing exactly how the machine identifies itself at runtime.

Compared with the other candidate milestones:

- `v0.3.0-settings-and-logging-foundation` is useful soon, but it does not validate the discovery boundary.
- `v0.3.0-capability-profile-static-loader` risks encoding assumptions before runtime identity is available.
- HP WMI, fan control, telemetry, and UI work remain intentionally premature.

## Files And Projects To Create Or Modify

Expected implementation scope:

- `src/VictusControl.Infrastructure/Windows/WindowsDeviceIdentityProvider.cs`
- `src/VictusControl.Infrastructure/Windows/IWindowsDeviceIdentityReader.cs` if a test seam is needed
- `src/VictusControl.Infrastructure/Windows/WindowsDeviceIdentitySnapshot.cs` if raw Windows fields need a small internal DTO
- `src/VictusControl.Infrastructure/VictusControl.Infrastructure.csproj`
- `tests/VictusControl.Infrastructure.Tests/` if the solution does not yet have infrastructure tests
- `VictusControl.sln` only if adding a new test project
- existing application tests only if constructor wiring or contracts require adjustment

The provider should map read-only Windows identity fields into `DeviceIdentity`. It may classify `HP` plus Victus-like model/product strings as `DeviceManufacturer.Hp` and `DeviceFamily.Victus`, but model matching must not mark fan, thermal, lighting, telemetry, or BIOS capabilities as supported.

## Out Of Scope

The milestone must not include:

- HP WMI control classes, namespaces, method calls, or write paths
- fan speed, fan curve, thermal profile, lighting, EC, or BIOS control
- telemetry polling or sensor integrations
- service/process conflict detection beyond a future-facing note
- settings persistence, logging framework, diagnostics export, update checks, tray behavior, or UI features
- vendor DLLs, HP binaries, drivers, native interop, or copied reference source
- reference repository modifications

## Tests To Add

Add focused tests that use fake readers or snapshots instead of touching the real machine:

- maps HP/Victus identity strings to `DeviceManufacturer.Hp` and `DeviceFamily.Victus`
- preserves model, SKU, product name, BIOS version, and manufacturer text
- maps unknown or empty manufacturer/model data to safe unknown identity values
- handles non-HP machines without marking them as Victus
- handles read failures by returning `DeviceIdentity.Unknown` or a clearly safe identity result
- proves the identity provider does not create or imply supported fan, thermal, telemetry, or write-capable capabilities

Do not add hardware tests requiring the target laptop, administrator rights, vendor services, HP software, or WMI write access.

## Verification Commands

Run the narrow standard verification after implementation:

```text
dotnet restore VictusControl.sln
dotnet build VictusControl.sln
dotnet test VictusControl.sln --no-build
```

If the solution file name changes or `.slnx` is adopted later, use the actual solution file and update `SESSION_STATE.md`.

## Reference Repositories

Relevant references:

- `seerge/g-helper`: only for lightweight Windows utility boundaries if startup wiring becomes relevant.
- `ib-3/ghelper-omen`: only for names of identity/capability concepts if a targeted comparison is explicitly needed.
- `theantipopau/omencore`: only for discovery safety patterns if a targeted comparison is explicitly needed.

References not needed:

- `breadeding/OmenSuperHub`: not needed for read-only identity mapping.
- `MasonDye/OmenXHub`: not needed for read-only identity mapping.
- `affaan-m/ECC`: not needed for product implementation.

Use VictusControl contracts first. Inspect reference repositories only for a specific question, record commit SHAs if used, and do not copy source.

## Risks

- Accidentally treating model detection as capability support.
- Introducing WMI package dependencies before deciding whether standard Windows APIs are enough.
- Making tests depend on the developer machine instead of fake input.
- Overfitting to one exact model string and failing nearby Victus 16 naming variants.
- Letting infrastructure leak Windows-specific types into `VictusControl.Domain`.

## Rollback Plan

Because this milestone is read-only and isolated, rollback should be simple:

- revert the new Windows identity provider files
- remove any new infrastructure test project from the solution
- restore project reference changes if they were added only for this milestone
- keep existing `v0.2.0` domain and abstraction contracts intact

No hardware state should be changed by the milestone, so no device restore action should be required.

## Recommended Model

Use `GPT-5.5 Medium`.

Reason: the task is multi-project and test-oriented, but still narrow and read-only. Escalate to `GPT-5.5 High` only if Windows API selection or failure behavior becomes ambiguous.

## Recommended Commit Message

```text
feat: add Windows device identity provider
```

## Immediately After This Milestone

After `v0.3.0-windows-device-identity-provider`, update `SESSION_STATE.md` with the provider, tests, and verification result.

The next planning decision should choose between:

- `v0.4.0-settings-and-logging-foundation`
- `v0.4.0-read-only-capability-profile-builder`

Do not move to HP WMI writes, fan control, telemetry loops, or UI controls until identity, logging, settings, and read-only capability profile behavior are in place.
