# Windows Packaging Readiness Audit

## Current Naming and Output

- The solution, project, assembly, executable, and ZIP target use `VictusX` / `VictusX.exe`.
- The manifest identity is `VictusX.app`, x64 is the only configured platform, and the app runs `asInvoker`.
- Internal `GHelper` root namespace, startup object, resource names, and single-instance event naming remain from the imported base.
- Explicit VictusX metadata now defines product, read-only diagnostic description, contributor attribution, copyright, and coherent `0.1.0-preview.1` / `0.1.0.0` version fields.

## Branding and Metadata Risks

- `favicon.ico` is still the inherited blue G icon, so Explorer, shortcuts, and executable properties do not present distinct VictusX branding.
- The project now supplies explicit VictusX product, description, company/authors, copyright, assembly, file, and informational version metadata. A future packaged smoke test must still confirm the final executable properties match those values.
- G-Helper and ASUS strings/resources remain in the binary. HP shell isolation hides inherited control/update surfaces, but packaging must not imply that those features support HP hardware.
- GPLv3 is present, but a preview package still needs clear source, attribution, modified-project, and third-party notice handling.
- `docs/third-party-notices-audit.md` now records the repository evidence: GPLv3 text and README-level G-Helper credit exist, while a versioned third-party notice inventory and resolved package-license review remain outstanding.
- `docs/dependency-notice-inventory.md` records the current local restore graph: 11 application packages and 13 test-only packages. It intentionally leaves license and notice status unreviewed pending authoritative metadata.

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

- Confirm the explicit product/version/publisher metadata in a packaged executable and add a distinct VictusX icon.
- Choose and document framework-dependent versus self-contained runtime packaging.
- Add a dedicated HP diagnostic publish profile and versioned artifact name.
- Provide a shortcut or launcher that always supplies only `--hp-victus`.
- Confirm inherited control/update surfaces remain unreachable in the packaged HP entry path.
- Verify clean-machine startup, report/export paths, missing-runtime behavior, upgrade/uninstall behavior, and clean shutdown.
- Run build/tests and a packaged smoke test with every explicit invocation field `Attempted=false`.
- Add license/third-party notices, checksums, malware scan results, and a signing plan before public distribution.
- Review the third-party notices audit, preserve applicable upstream notices, and verify direct and transitive package attribution from authoritative metadata.
- Review and complete the dependency notice inventory against a clean restore and the final package contents.
- Confirm no developer-only flags, symbols, logs, machine paths, or captured device data are shipped.

## Change Now

Keep the explicit VictusX metadata aligned with the future artifact version, then replace the inherited icon and complete the packaged smoke-test checklist before any release work.

See [Third-Party Notices Audit](third-party-notices-audit.md) for the source-attribution and package-notice distribution gate.

## Wait Until Later

Defer automatic updates, startup registration, a production installer/store package, fan/performance UI claims, hardware-control permissions, and any control-oriented shortcut until the corresponding HP behavior is implemented and safety-validated. Do not globally rename internal namespaces or remove default G-Helper behavior as part of preview packaging.

## Recommended Next Safe Task

Design the dedicated HP diagnostic publish profile and preview artifact contract as documentation and MSBuild metadata only; do not publish binaries yet.

See [HP Diagnostic Publish Profile Design](hp-diagnostic-publish-profile-design.md) for the proposed artifact name, dedicated launcher contract, deployment choice, and fail-closed pre-release checklist.
