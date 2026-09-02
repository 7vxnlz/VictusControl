# HP Diagnostic Preview Release Blockers

## Current Release Status

The VictusX HP diagnostic preview is not release-ready. HP diagnostic mode is stable and read-only, but distribution remains blocked until notice, identity, signing, checksum, and clean-machine validation evidence is complete.

No binaries or release artifacts should be published from the current state.

See [HP Diagnostic Preview Source Readiness Audit](hp-diagnostic-preview-source-readiness-audit.md) for the final source-only readiness summary.

See [HP Diagnostic Preview Readiness Checkpoint](hp-diagnostic-preview-readiness-checkpoint.md) for the post-proof-gap-analyzer preview state, including read-only fan proof status, close-to-tray behavior, and the remaining packaging blockers.

## Completed Prerequisites

- HP diagnostic mode launches with `--hp-victus` and uses a Diagnostic/Quit-only shell.
- HP read-only diagnostic dashboard is report-backed and local-file-only.
- User-facing safety notes exist for the future preview entry path.
- Final source-only readiness audit exists for the current preview package state.
- HP capability report schema metadata persists.
- Publish profile and launcher source files exist for a future HP diagnostic preview.
- Safety tests guard the launcher/profile from including `--hp-wmi-readonly-test`.
- Project metadata is aligned to VictusX preview identity while keeping inherited default behavior intact.
- Dependency notice inventory, icon/app identity plan, and clean-machine validation plan exist.
- HP fan proof-gap analyzer and preview readiness checkpoints exist after dashboard stabilization.

## Blocking Items

- Complete package-license and notice review from authoritative package metadata.
- Replace inherited G-Helper visual identity with approved VictusX icon/app assets.
- Define and verify signing and checksum output for the final package.
- Run the clean-machine validation plan against a final candidate package.
- Confirm the final package contains no logs, captured device data, symbols, machine paths, or developer-only flags.
- Confirm the release entry point always launches with only `--hp-victus`.
- Include or link [HP Diagnostic Preview User Safety Notes](hp-diagnostic-preview-user-safety-notes.md) with the preview package.

## Package-License/Notice Review Blocker

Use [Third-Party Notices Audit](third-party-notices-audit.md), [Dependency Notice Inventory](dependency-notice-inventory.md), and [Package License Review Workflow](package-license-review-workflow.md) as the current source material. The blocker remains open until direct and transitive package notices are reviewed against authoritative package metadata and matched to final package contents.

The future ZIP/installer must include applicable license text, upstream G-Helper modified-project attribution, and reviewed third-party notices.

## Icon/App Identity Blocker

Use [VictusX Icon and App Identity Plan](victusx-icon-app-identity-plan.md) and [VictusX Icon Asset Requirements](victusx-icon-asset-requirements.md). The inherited icon remains a release blocker until an original or properly licensed VictusX icon is added and verified for executable properties, Explorer, shortcuts, tray, and window display.

Default ASUS/G-Helper behavior and shared resources must not be broken while preparing HP diagnostic preview identity.

## Signing/Checksum Blocker

A preview package needs at least a verified SHA-256 checksum. Authenticode signing should be added when a verified publisher identity is available. Follow [Signing and Checksum Workflow](signing-checksum-workflow.md); the checksum/signing process must be repeatable and must apply to the final package actually tested.

## Clean-Machine Validation Blocker

Use [Clean-Machine Validation Plan](clean-machine-validation-plan.md). This blocker remains open until a final package is tested on a clean Windows machine or VM using the launcher only, with no prior app data required and no developer-only flags.

Validation must confirm startup, report path behavior, export behavior, Diagnostic/Quit-only shell, clean shutdown, no remaining process, and no crash records.

## Safety Blockers That Must Remain Closed

- Fan control must remain unimplemented.
- Fan writes must remain unimplemented.
- SetFanMax must remain NO-GO/design-only.
- Performance control must remain unimplemented.
- Power-limit writes, EC writes, BIOS writes, and hardware-control paths must remain absent.
- `--hp-wmi-readonly-test` must not appear in release launchers, shortcuts, publish profiles, or user-facing release instructions.

These are intentional safety conditions, not release tasks to unblock.

## Required Final Pre-Release Evidence

- Final package contents list.
- Confirmed launcher arguments: exactly `--hp-victus`.
- Confirmed absence of `--hp-wmi-readonly-test` and future write/control flags.
- Build and test results from the release candidate source.
- Dependency and notice review result following the package license review workflow.
- Icon/app identity verification result against the asset acceptance checklist.
- SHA-256 checksum and signing status following the signing/checksum workflow.
- Clean-machine validation record.
- Report showing explicit read-only invocations remain `Attempted=false` during normal HP diagnostic mode.
- Manual confirmation that no fan/performance/write UI is present.
- Final user-facing safety notes reviewed for the exact release candidate.

## Recommended Next Safe Task

Create a source-only `THIRD-PARTY-NOTICES.md` draft from the current dependency inventory, leaving license decisions marked for review and publishing still blocked.
