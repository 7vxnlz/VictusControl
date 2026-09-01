# SetFanMax Proof Gap Checklist

This is the source-of-truth checklist for evidence required before any SetFanMax write implementation may begin. It records proof status only and never authorizes a write.

## Current Status

**NO-GO.** SetFanMax is not implemented or enabled. `DeviceValidatedInputLength` remains unset, and no code may default, infer, probe, retry, or fall back between payload lengths.

## Known Command Evidence

The reviewed references consistently support the following metadata, but not this device's complete write contract:

- Command: `0x20008`
- Command type: `0x27`
- Method: `hpqBIOSInt0`
- Enable state byte: `0x01`
- Restore/disable state byte: `0x00`

## Payload-Length Gap

| Candidate | Existing evidence | Missing exact-device proof |
| --- | --- | --- |
| 1 byte | OmenSuperHub and OmenXHub use the state byte alone. | No validated SetFanMax trace for Victus `16-s0035nt`, SKU `7Z5Z2EA#AB8`, BIOS `F.31`. |
| 4 bytes | ghelper-omen and omencore use `{ state, 0, 0, 0 }`; omencore has the closest V1 Victus 16-s0xxx cohort. | The closest cohort is BIOS `F.30` and `UserVerified=false`; no exact F.31 validation exists. |

Read-only success from `FanMaxGet`, `FanGetCount`, or `FanGetLevel` does not establish the write input length. Neither candidate is selected.

## Remaining Proof Gaps

- [ ] **Device input ABI:** one independently reviewable record selects exactly one input length for this exact model/SKU/BIOS, with no alternate-shape fallback.
- [ ] **Restore behavior:** the same selected length restores max fan to disabled, proven by successful `FanMaxGet` readback matching the captured baseline.
- [ ] **Thermal observation:** a named independent source, baseline, stop thresholds, observer, and continuous availability through restore and recovery are reviewed.
- [ ] **AC, battery, and power state:** stable AC, adequate battery reserve, and defined behavior for unplug, suspend, shutdown, or power transition are evidenced for the experiment. Existing policy requirements are not device proof.
- [ ] **Failure and recovery:** exception, timeout, cancellation, ambiguous readback, lost power, and failed restore have a reviewed stop path and a locally available manual recovery procedure.
- [ ] **Rollback proof:** one complete evidence chain records baseline, bounded enable observation, restore, final `FanMaxGet=false`, baseline match, and recovery outcome.
- [ ] **Human review:** the evidence package and implementation-only approval wording are recorded for this exact device and BIOS.

## Required Before Implementation

Every checkbox above must be proven and independently reviewable. The evidence must also identify the exact method, command, command type, selected input size, bounded target, pre/post/restore readbacks, return/error metadata, timing, AC/battery state, thermal observations, aborts, and recovery result. A separate task must then explicitly authorize a constrained implementation design; proof completion alone does not authorize execution.

SetFanMode, SetFanLevel, ambiguous `0x37`, EC paths, generic write APIs, automatic retry, background operation, and payload-shape fallback cannot be used to fill any gap.

## Required Before UI Exposure

Even after a separately authorized guarded experiment, fan UI remains blocked until repeated exact-device evidence proves safe enable, restore, cancellation, shutdown, suspend/resume, power-loss handling, and recovery across reviewed runs. A product-level safety review must separately approve bounded controls, lifecycle behavior, error reporting, and removal of write access when evidence becomes stale. One successful experiment is not UI evidence.

## No-Guess Rule

No code should guess `1` or `4`, choose by repository count or model similarity, construct both shapes, retry with the other shape, or silently default a missing value. `DeviceValidatedInputLength` must remain unset and preflight must fail closed until exact-device evidence selects one length.

## Recommended Next Safe Task

Perform documentation-only collection and independent review of an existing sanitized SetFanMax field record for Victus `16-s0035nt` / `7Z5Z2EA#AB8` / BIOS `F.31`. It must include exact input size plus `FanMaxGet` baseline, post-action, restore, thermal/power, and recovery evidence. If no complete record exists, stop and retain **NO-GO**; do not generate the missing evidence through VictusX.

Supporting detail: [payload-length final audit](set-fan-max-payload-length-final-audit.md), [recovery/restore proof plan](set-fan-max-recovery-restore-proof-plan.md), and [implementation gate](set-fan-max-implementation-go-no-go.md).
