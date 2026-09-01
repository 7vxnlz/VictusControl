# Windows Packaging Readiness Audit

## Current Naming and Output

- The solution, project, assembly, executable, and ZIP target use `VictusX` / `VictusX.exe`.
- The manifest identity is `VictusX.app`, x64 is the only configured platform, and the app runs `asInvoker`.
- Internal `GHelper` root namespace, startup object, resource names, and single-instance event naming remain from the imported base.
- Assembly version `0.279` still resembles the upstream G-Helper version and is not a defined VictusX preview version.

## Branding and Metadata Risks

- `favicon.ico` is still the inherited blue G icon, so Explorer, shortcuts, and executable properties do not present distinct VictusX branding.
- Windows currently derives `VictusX` for product, company, and description, but these values are not explicitly defined. The built executable reports file version `0.279`, product version `1.0.0+<git hash>`, and no copyright, so preview version identity is inconsistent.
- G-Helper and ASUS strings/resources remain in the binary. HP shell isolation hides inherited control/update surfaces, but packaging must not imply that those features support HP hardware.
- GPLv3 is present, but a preview package still needs clear source, attribution, modified-project, and third-party notice handling.

## Package and Installer Status

- The only publish profile creates a framework-dependent, single-file `win-x64` build; the .NET 10 Windows Desktop Runtime is therefore required.
- Publish currently creates an unversioned `VictusX.zip`.
- No installer/MSIX/WiX/Inno Setup project, release workflow, code signing, checksum generation, update channel, uninstall behavior, or packaged shortcut definition exists.
- Imported hardware-oriented resources and dependencies remain in the executable even though HP Diagnostic mode does not use their control paths.

## Required HP Diagnostic Runtime

A diagnostic preview must launch with exactly:

```text
--hp-victus
```

The IDE launch profile is not embedded into a published executable. Any preview shortcut or launcher must explicitly include this flag. A bare executable preserves the default G-Helper/ASUS behavior and is not a safe HP diagnostic preview entry point.

`--hp-wmi-readonly-test` must never appear in release profiles or shortcuts. It enables a separate elevated developer-only probe gate and is not required for the cached diagnostic shell.

## Safety Status

- HP mode is diagnostic-only and report-backed.
- Fan and performance control are not implemented.
- Fan writes are not implemented.
- SetFanMax remains NO-GO/design-only.

## Pre-release Checklist

- Add a distinct VictusX icon and explicit product/version/publisher metadata.
- Choose and document framework-dependent versus self-contained runtime packaging.
- Add a dedicated HP diagnostic publish profile and versioned artifact name.
- Provide a shortcut or launcher that always supplies only `--hp-victus`.
- Confirm inherited control/update surfaces remain unreachable in the packaged HP entry path.
- Verify clean-machine startup, report/export paths, missing-runtime behavior, upgrade/uninstall behavior, and clean shutdown.
- Run build/tests and a packaged smoke test with every explicit invocation field `Attempted=false`.
- Add license/third-party notices, checksums, malware scan results, and a signing plan before public distribution.
- Confirm no developer-only flags, symbols, logs, machine paths, or captured device data are shipped.

## Change Now

Define VictusX product metadata and icon requirements, then add a separate HP diagnostic publish profile whose artifact and launch instructions require `--hp-victus`. Keep it non-releasing until the packaged smoke-test checklist passes.

## Wait Until Later

Defer automatic updates, startup registration, a production installer/store package, fan/performance UI claims, hardware-control permissions, and any control-oriented shortcut until the corresponding HP behavior is implemented and safety-validated. Do not globally rename internal namespaces or remove default G-Helper behavior as part of preview packaging.

## Recommended Next Safe Task

Design the dedicated HP diagnostic publish profile and preview artifact contract as documentation and MSBuild metadata only; do not publish binaries yet.
