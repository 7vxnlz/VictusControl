# Fan Write Blocker Summary

## 1. Current Read-Only Successes

The HP Victus read-only path is stable and report-backed:

- `SystemDesignData` succeeded and decoded `ThermalPolicyVersion=1` plus a software fan-control support declaration.
- `FanGetCount` succeeded and reports two fans with protection status clear.
- `FanMaxGet` succeeded and reports max fan disabled.
- `FanGetLevel` succeeded, but its returned values remain raw-only and must not be treated as RPM, percent, a curve point, or a writable target.

These results prove diagnostic visibility only. They do not validate any write command or payload.

## 2. Why SetFanMax Is Blocked

SetFanMax (`0x27`) remains blocked because its device-specific input length is unknown. Two exact-device four-byte attempts and one one-byte comparison returned successful calls and observed fan response, but FanMaxGet remained false after enable and restore; see the [one-byte comparison result](set-fan-max-1byte-comparison-result.md). Four byte has more repeated experimental evidence, but neither is a validated payload or reliable state readback. Matching enable/disable behavior, restore proof, and manual recovery proof remain incomplete.

`DeviceValidatedInputLength` therefore remains unset and normal SetFanMax control remains **NO-GO**. The repeated four-byte physical response permits only separately approved, command-line-only developer experiments. One byte may be used only for one separately approved comparison experiment; neither path permits normal writes or UI control.

Experiment logs may classify a successful command plus manually observed fan response and `FanMaxGet=false` as readback-inconclusive. In that narrow case, legacy `Outcome` is `Unknown` rather than a misleading plain `Fail`, and the FanMaxGet-only failure reason is omitted. This distinction does not validate a payload, recover a reliable latch readback, or permit normal fan control.

The [clean classified four-byte result](set-fan-max-4byte-clean-classified-result.md) records this mapping on the exact device. It supports a limited developer-only pulse-design discussion, not a normal control path.

The [four-byte Max Fan Pulse design](set-fan-max-4byte-max-fan-pulse-design.md) has a separately gated command-line implementation, but it remains unexecuted and has no normal UI route. It does not alter normal control readiness.

The [pulse result](set-fan-max-4byte-max-fan-pulse-result.md) now records a successful bounded developer run. It makes the developer pulse operational, not normal fan control.

Developer experiment observation arguments are log metadata only. They require the existing command-line experiment path and cannot approve a payload, bypass a one-byte or four-byte approval, or expose a user-facing control.

## 3. Why SetFanMode Is Blocked

SetFanMode (`0x1A`) changes thermal-policy state rather than a simple fan flag. References disagree between two-byte and four-byte inputs, and mode values vary by thermal-policy generation. Reference flows also show that a successful return may not mean the hardware completed the transition. VictusX has no validated mode readback, baseline mode, restore sequence, or exact-device payload contract.

## 4. Why SetFanLevel Is Blocked

SetFanLevel (`0x2E`) has the widest uncertainty: reviewed references use two-, three-, four-, and 128-byte inputs. Fan ordering, capability bits, and value scale differ across platforms. V1 references document problematic handoffs, including maximum, zero-speed, or non-responsive behavior after `SetFanLevel(0,0)`. The working `FanGetLevel` read is raw-only and cannot validate write semantics or prove a return to BIOS automatic control.

## 5. Why 0x37 Is Blocked

`0x37` is ambiguous across references. It appears as a V2/OMEN Max fan-level read path in some code and as write-like power-limit/control behavior in other command flows. This device reports thermal policy V1, and no device-specific method, direction, payload, or safe interpretation has been established. It must not be probed, prepared as a write, or used as a fallback.

## 6. Why Fan UI Control Must Wait

A control UI would imply that a command, range, state transition, restore path, and failure response are trustworthy. None is proven. Buttons, toggles, sliders, curves, persistence, retries, or background control could turn unresolved firmware behavior into repeated hardware writes. The existing HP UI must remain read-only and cached/report-backed.

## 7. Exact Missing Evidence Before Any Write

Before any fan write can even be reconsidered, all of the following must exist for one exact command on this exact model/SKU/BIOS:

1. One independently reviewed method, command, input size, payload meaning, and bounded target state, with no alternate-shape fallback.
2. A known-safe readback proving the pre-write baseline and post-write state independently of the write return code.
3. A length- and command-matched restore action followed by readback proving the original state was recovered.
4. A reviewed manual recovery procedure for failed, ambiguous, or persistent firmware state.
5. Defined AC-power, thermal-observation, abort, timeout, and cancellation conditions.
6. Explicit human approval scoped to implementation design first; execution would require separate authorization.
7. Evidence that the selected command does not depend on an unvalidated SetFanMax, SetFanMode, SetFanLevel, `0x37`, or EC side path.

Until every item is proven, all fan writes remain **NO-GO**.

## 8. Recommended Next Safe Task

The [first-write experiment runner design](set-fan-max-first-write-experiment-runner-design.md) now has a developer-only, command-line-only implementation. Its explicit four-byte-only approval flag does not select the payload, change `DeviceValidatedInputLength`, or enable normal control. The next safe task is a separately authorized controlled second four-byte confirmation design with stronger manual evidence, not a one-byte test or UI work.

Supporting decisions: [SetFanMax payload-length final audit](set-fan-max-payload-length-final-audit.md), [SetFanMode and SetFanLevel risk study](set-fan-mode-level-risk-study.md), and [missing-proof tracker](set-fan-max-missing-proof-tracker.md).

The authoritative current gap list is the [SetFanMax proof gap checklist](set-fan-max-proof-gap-checklist.md). It keeps payload length unset and all fan writes **NO-GO** until exact-device restore, thermal/power, recovery, rollback, and approval evidence is complete.

The [manual evidence capture package](set-fan-max-manual-evidence-capture.md) defines the required record format. It is documentation-only and does not permit VictusX to generate missing write evidence.

The [SetFanMax first-write decision gate](set-fan-max-first-write-decision-gate.md) consolidates the required proof into one explicit NO-GO threshold before any implementation-design task may begin.

The focused [payload-length reference decision](set-fan-max-payload-length-reference-decision.md) confirms that neither reference shape is exact-device evidence; no payload length or write candidate has been selected.

The [manual experiment logger design](set-fan-max-manual-experiment-logger-design.md) is documentation-only and cannot bypass any blocker, enable a command, or justify fan UI control.

The [first-write runner safety audit](set-fan-max-first-write-runner-safety-audit.md) verifies the implemented runner remains command-line-only, approval-gated, and unable to alter this NO-GO status.

The [one-byte comparison result](set-fan-max-1byte-comparison-result.md) shows a similar physical response to the two four-byte records, but retains NO-GO for normal control. The [payload strategy decision](set-fan-max-payload-strategy-decision.md) keeps four byte preferred for controlled confirmation and one byte comparison-only. Any future experiment still requires separate approval, append-only evidence logging, and matching restore; it is not payload validation or UI-control authorization.

That metadata must explicitly retain unreliable readback, unvalidated normal control, and prohibited user-facing control. It records evidence; it cannot change a safety decision.
