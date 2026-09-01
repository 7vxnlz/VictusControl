# HP Diagnostic Preview Artifact Checklist

## Purpose

`app/Properties/PublishProfiles/VictusX-HP-Diagnostic-win-x64.pubxml` is the source-side profile for a future self-contained `win-x64` HP diagnostic preview. It is not a release instruction and does not publish an artifact by itself.

`tools/run-victusx-hp-diagnostic.ps1` is the future portable-package launcher source. When placed under a package `tools` folder beside `VictusX.exe`, it starts the application with exactly:

```text
--hp-victus
```

## Required and Forbidden Arguments

- Required runtime argument: `--hp-victus`.
- Forbidden from the profile, launcher, shortcuts, and release instructions: `--hp-wmi-readonly-test` and all future write/control arguments.
- The normal diagnostic shell is local/report-backed; explicit developer probes are not part of a preview artifact.

## Before Any Preview Publish

- Confirm the explicit VictusX version, product, publisher, and copyright metadata in the packaged executable; replace the inherited icon and add license and notice metadata.
- Confirm the launcher is packaged under `tools` beside `VictusX.exe` and that its only application argument is `--hp-victus`.
- Run a clean-machine packaged smoke test: diagnostic-only shell, no elevation, no control/update surfaces, and every explicit invocation field `Attempted=false`.
- Verify report loading and local-only copy, reload, folder-open, and export behavior for missing and corrupt reports.
- Produce and verify a SHA-256 checksum. Add timestamped Authenticode signing when a verified publisher identity is available.
- Exclude logs, raw device captures, machine-specific paths, and symbols from distribution.

## Repository Safety Checks

`HpDiagnosticPreviewConfigurationTests` reads only the launcher's and publish profile's source text. It verifies that the launcher supplies `--hp-victus`, neither source file contains `--hp-wmi-readonly-test`, and the profile opts out of the inherited ZIP-on-publish target. The tests do not execute the launcher or invoke `dotnet publish`.

## Release Status

Release remains blocked. No fan or performance control, fan writes, or hardware-control support is included or claimed. SetFanMax remains NO-GO/design-only.
