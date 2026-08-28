# Next Milestone Plan

This plan defines the next safe milestone after `v0.3.0-windows-device-identity-provider`. It is a planning document only and does not add implementation code, HP WMI calls, fan control, telemetry loops, EC access, BIOS writes, UI features, or hardware write logic.

## Recommended Milestone

Milestone: `v0.4.0-settings-and-logging-foundation`

Create the minimal settings and logging foundations needed before VictusControl expands read-only hardware discovery beyond identity. The milestone should define safe configuration boundaries, privacy-aware logging contracts, and simple infrastructure implementations without introducing hardware control.

## Why This Comes Next

`v0.3.0` added a read-only Windows identity provider. The next temptation is to keep adding discovery sources, but broader discovery becomes much easier to debug, test, and share safely if the project first has clear logging and settings boundaries.

This milestone comes before the other candidate milestones because:

- `v0.4.0-static-capability-profile-loader` would encode capability decisions before the app has enough runtime evidence and diagnostics.
- `v0.4.0-read-only-system-snapshot` is useful soon, but it should produce structured logs and obey settings for diagnostics detail before it collects more machine facts.
- HP WMI capability probing should wait until the app can record what was probed, what failed, and what was intentionally skipped without leaking private or noisy machine data.

Settings and logging are still safe because they do not touch hardware, services, BIOS, EC, fans, telemetry loops, or HP-specific control paths.

## Files And Projects To Create Or Modify

Expected implementation scope:

- `src/VictusControl.Application/Settings/` for settings-facing application contracts if needed
- `src/VictusControl.Application/Diagnostics/` for logging or diagnostic event contracts if needed
- `src/VictusControl.Infrastructure/Settings/` for a minimal local settings store
- `src/VictusControl.Infrastructure/Diagnostics/` for a simple file or in-memory logger
- `src/VictusControl.Infrastructure/VictusControl.Infrastructure.csproj` only for necessary project references or framework-supported APIs
- `tests/VictusControl.Application.Tests/` for settings/logging contract behavior
- `tests/VictusControl.Infrastructure.Tests/` for persistence and logging implementation tests
- `SESSION_STATE.md` after implementation verification
- `AI_CONTEXT.md` only if the architecture description changes meaningfully

Keep any persisted settings schema small. A first version may include only diagnostics/logging preferences and app-level safety defaults. Avoid feature settings for fans, thermal modes, keyboard lighting, HP WMI, or telemetry until those features are designed.

## Out Of Scope

The milestone must not include:

- HP WMI classes, namespace probing, method calls, or control logic
- fan control, fan curves, thermal modes, keyboard lighting, EC access, BIOS writes, or telemetry loops
- read-only system inventory expansion beyond what tests require for settings/logging
- UI settings screens, tray behavior, startup registration, update checks, packaging, or installers
- vendor DLLs, HP binaries, drivers, native interop, or copied reference source
- automatic log upload, crash reporting services, analytics, or external telemetry
- reference repository modifications

## Tests To Add

Add focused tests with fake clocks, fake file systems, temporary directories, or in-memory sinks as appropriate:

- default settings are safe, minimal, and deterministic
- settings load returns defaults when no file exists
- invalid or unreadable settings fail safely without throwing into application startup
- settings save/load round-trips only the approved schema
- logging captures level, timestamp/source if designed, message, and optional safe properties
- logging avoids raw dumps and supports sanitized diagnostic events
- infrastructure tests do not require HP hardware, admin rights, vendor services, or machine-specific paths

Do not add tests that require real hardware, HP software, WMI writes, fan state, BIOS state, EC access, network access, or application execution.

## Verification Commands

Run the standard verification after implementation:

```text
dotnet restore VictusControl.sln
dotnet build VictusControl.sln
dotnet test VictusControl.sln --no-build
```

If package additions are proposed, justify them first. Prefer built-in .NET APIs for JSON and file I/O unless a small dependency is clearly worth it.

## Reference Repositories

Relevant references:

- `seerge/g-helper`: optional targeted reference for lightweight settings and update-free app ergonomics.
- `ib-3/ghelper-omen`: optional targeted reference for how HP/Omen-facing behavior logs or stores capability-related state.
- `theantipopau/omencore`: optional targeted reference for safety-oriented diagnostics and tests.

References not needed:

- `breadeding/OmenSuperHub`: not needed for a minimal settings/logging foundation.
- `MasonDye/OmenXHub`: not needed unless a later WPF settings UX task is scoped.
- `affaan-m/ECC`: not needed for product implementation.

Use VictusControl files first. Inspect reference repositories only for a specific settings/logging question, record commit SHAs if used, and do not copy source.

## Risks

- Overbuilding a logging framework before real diagnostics needs are proven.
- Persisting machine-specific or privacy-sensitive data by default.
- Letting settings become a dumping ground for future hardware behavior.
- Adding dependencies that create avoidable maintenance or licensing obligations.
- Making infrastructure tests depend on user profile paths, locale-specific messages, or the real machine state.

## Rollback Plan

Rollback should be straightforward:

- remove the new settings and diagnostics contracts
- remove the concrete infrastructure settings/logger implementations
- remove the matching tests
- revert any project reference or package changes added only for this milestone
- keep the existing solution skeleton, domain contracts, and Windows identity provider intact

No hardware state should be changed by this milestone, so no device restore action should be required.

## Recommended Model

Use `GPT-5.5 Medium`.

Reason: the task crosses Application, Infrastructure, and tests, but it is not hardware-control work. Use `GPT-5.5 Low` only if the implementation is limited to simple interfaces and in-memory tests; escalate to `GPT-5.5 High` only if settings migration, privacy rules, or logging schema tradeoffs become non-trivial.

## Recommended Commit Message

```text
feat: add settings and logging foundation
```

## Preparing For Later HP WMI Capability Probing

This milestone prepares for HP WMI capability probing without adding HP WMI by establishing:

- a place to record which read-only probes ran and which were skipped
- privacy-aware diagnostic events for unavailable or unsupported capabilities
- settings that can later control diagnostics verbosity and safe defaults
- tests proving failure paths stay quiet, deterministic, and non-destructive

Later HP WMI capability probing should use these foundations to report evidence and reasons, not to enable controls automatically. Even after HP-specific namespaces are discovered, fan control, thermal control, EC access, and BIOS writes must remain separate explicitly approved milestones.

## Immediately After This Milestone

After `v0.4.0-settings-and-logging-foundation`, update `SESSION_STATE.md` with the contracts, implementations, tests, and verification result.

The next planning decision should choose between:

- `v0.5.0-read-only-system-snapshot`
- `v0.5.0-read-only-capability-profile-builder`

Do not move to HP WMI writes, fan control, telemetry loops, EC access, BIOS writes, or UI controls until the read-only discovery pipeline can produce explainable capability evidence.
