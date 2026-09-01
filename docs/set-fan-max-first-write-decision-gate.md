# SetFanMax First-Write Decision Gate

## Current Status

**NO-GO.** This gate is not satisfied. SetFanMax is not implemented or enabled, `DeviceValidatedInputLength` is unset, and VictusX must not add write code until this document is updated to **GO** with cited evidence.

## Known Evidence

- The exact target is HP Victus `16-s0035nt`, SKU `7Z5Z2EA#AB8`, BIOS `F.31`, thermal policy V1, with two fans.
- Read-only `SystemDesignData`, `FanGetCount`, `FanMaxGet`, and raw-only `FanGetLevel` succeeded.
- References support `hpqBIOSInt0`, command `0x20008`, type `0x27`, enable byte `0x01`, and restore/disable byte `0x00`.

## Required GO Evidence

Every item must be independently reviewable and traceable to the exact device and BIOS above:

1. **Payload length:** one exact-device record proves either the one-byte or four-byte shape, with no guess, fallback, retry, or alternate-shape experiment.
2. **Restore:** the selected shape has a matching restore/disable result and final `FanMaxGet=false` readback matching the baseline.
3. **Thermal and power state:** stable AC, documented battery reserve, independent thermal baseline/limits, and defined stop behavior for power transitions are evidenced through restore.
4. **Failure and recovery:** timeout, exception, cancellation, ambiguous readback, power loss, and failed restore each have a reviewed abort and locally available recovery path.
5. **Human approval:** an independent reviewer records the evidence references, exact identity, conflicts, and approval for a separately scoped implementation-design task.

## If The Gate Later Becomes GO

Any future implementation remains constrained to SetFanMax only, one bounded target action, exact validated input length, pre/post/restore `FanMaxGet` readbacks, fail-closed aborts, no retry/fallback, no background operation, and no UI exposure. SetFanMode, SetFanLevel, `0x37`, EC paths, performance control, and fan controls remain outside scope.

## Decision

The gate is currently **not satisfied**. No write code, payload selection, or hardware experiment may be added until this document is explicitly changed to **GO** with the required evidence. The next safe task is documentation-only review of an existing sanitized exact-device field record; if none exists, retain NO-GO.

The cached HP diagnostic report and dashboard expose this decision as `SetFanMaxFirstWriteGateStatus=NO-GO`, `SetFanMaxFirstWriteGateSatisfied=false`, and a fail-closed reason. These fields are diagnostic-only and cannot authorize or execute a write.

Older cached reports that lack these fields must also display `NO-GO` / not satisfied with an explicit old-report missing-field reason. Cached optimistic values such as `GO` or `true` are treated as invalid for authorization and remain blocked.

Test coverage for these fail-closed dashboard, report, and copy/export summary paths is tracked in [set-fan-max-gate-test-coverage-checkpoint.md](set-fan-max-gate-test-coverage-checkpoint.md).

Any manually completed export must first pass the [manual evidence review workflow](set-fan-max-manual-evidence-review-workflow.md). Only a cited update of this gate and the proof gap checklist can change the current NO-GO decision.

A future import parser may help identify missing or conflicting fields, but is not evidence approval and cannot update this gate; see the [evidence import/review design](set-fan-max-evidence-import-review-design.md).
