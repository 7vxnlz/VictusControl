# SetFanMax Missing-Proof Tracker

This tracker records evidence required before SetFanMax may move beyond implementation `NO-GO`. Status describes current project evidence, not permission to write.

## Missing-Proof Table

| Proof item | Status | Exact evidence required for `Proven` | Supporting documents |
| --- | --- | --- | --- |
| Device-specific input length | **Missing** | Independent evidence for this exact model/BIOS selecting exactly one input length (`1` or `4`), with no fallback. | [Method/input validation](set-fan-max-method-input-validation.md), [device validation plan](set-fan-max-device-validation-plan.md) |
| Restore/disable behavior | **Missing** | Length-matched restore evidence and successful `FanMaxGet` proving disabled and matching baseline. | [Recovery/restore proof plan](set-fan-max-recovery-restore-proof-plan.md), [manual validation package](set-fan-max-manual-validation-package.md) |
| Manual recovery path | **Missing** | A reviewed local procedure for this model/BIOS, authoritative OEM source where relevant, and `FanMaxGet=false` after recovery. | [Recovery/restore proof plan](set-fan-max-recovery-restore-proof-plan.md), [write preflight checklist](hp-fan-write-preflight-checklist.md) |
| Human approval | **Missing** | Reviewer metadata and exact scoped approval wording, with runtime approval deferred until every gate is proven. | [Implementation gate](set-fan-max-implementation-go-no-go.md), [future runtime flag design](set-fan-max-future-runtime-flag-design.md) |
| Thermal observation plan | **Deferred** | Named source, observer, baseline, stop thresholds, visibility method, and availability through restore/recovery. | [Write safety design](hp-fan-write-safety-design.md), [future runtime flag design](set-fan-max-future-runtime-flag-design.md) |
| AC power requirement | **Proven** | Stable AC is a modeled and tested mandatory preflight gate; any future run must still record live AC and battery reserve. | [Write preflight checklist](hp-fan-write-preflight-checklist.md), [future runtime flag design](set-fan-max-future-runtime-flag-design.md) |
| Rollback proof | **Missing** | One reviewed chain showing baseline, enable observation, restore, final `FanMaxGet=false`, baseline match, and recovery outcome. | [Recovery/restore proof plan](set-fan-max-recovery-restore-proof-plan.md), [manual validation package](set-fan-max-manual-validation-package.md) |

All statuses remain unchanged by the runtime flag design. Proposed flags and approval wording are requirements, not proof and not authorization.

## Current Decision

**NO-GO.** `DeviceValidatedInputLength` remains `null`; restore, recovery, approval, and rollback proof are missing; and thermal observation is deferred.

## Still Forbidden

SetFanMax execution, write payload execution, `SetFanMode`, `SetFanLevel`, ambiguous `0x37`, automatic retry/restore, background writes, fan UI/control, EC access, BIOS writes, hardware writes, and changes to default ASUS behavior remain forbidden.

## Next Safe Task

Conduct a documentation-only human review of the proposed flag names against the existing placeholder policy, then choose one canonical set without runtime wiring. Device evidence statuses must not change until independently reviewable proof exists.
