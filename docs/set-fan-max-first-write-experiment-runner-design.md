# SetFanMax First-Write Experiment Runner Design

## Status

The developer-only runner is implemented, but the first-write gate remains **NO-GO**. The application supplies `IsFirstWriteGateApproved=false` and `HasReviewedHumanApproval=false`, so every current invocation stops after any permitted read-only baseline and writes a blocked log. `DeviceValidatedInputLength` is unset and neither payload length is validated. The implementation does not authorize execution.

## Proven Baseline

The exact target is Victus by HP Gaming Laptop `16-s0xxx`, SKU `7Z5Z2EA#AB8`, BIOS `F.31`, thermal policy V1. Elevated read-only baselines report `FanGetCount=2`, `FanMaxGet=false`, and raw FanGetLevel prefix `22-25`; `SystemDesignData`, `FanGetCount`, `FanMaxGet`, and `FanGetLevel` all succeeded. See [exact-device baseline evidence](set-fan-max-exact-device-baseline-evidence.md).

## Candidate Payloads

| Hypothesis | Enable | Matching restore/off |
| --- | --- | --- |
| Four byte | `01-00-00-00` | `00-00-00-00` |
| One byte | `01` | `00` |

Exactly one hypothesis may be supplied for one future run. The runner must never infer a default, try the other length, retry, or fall back. Supplying a hypothesis does not validate it.

## Future Flags And Gates

The future runner would require all of these flags:

- `--hp-victus`
- `--hp-wmi-readonly-test`
- `--hp-fan-write-experiment`
- `--set-fan-max-payload-length=1` or `=4`
- `--i-understand-this-can-affect-fans`

It also requires a separately reviewed first-write gate explicitly changed to GO, exact model/SKU/BIOS match, an elevated Administrator process, confirmed AC power, explicit human approval, and a same-session successful baseline. AC is checked through the local Windows power-line status API and unknown/offline power fails closed. That baseline must include all approved read-only probes, `FanGetCount=2`, `FanMaxGet=false`, and recorded raw FanGetLevel values. Any mismatch, unavailable reading, or missing approval blocks the run before a write.

## Future Run Sequence

1. Open an append-only experiment record and capture the exact identity, flags, power state, manual operator confirmation, baseline probes, and chosen hypothesis.
2. Recheck every gate immediately before the write boundary.
3. Execute one SetFanMax enable payload for the selected hypothesis only.
4. Wait one short, controlled observation interval, never exceeding ten seconds and ending immediately on a stop condition.
5. Read `FanMaxGet` and raw `FanGetLevel`; record return/error data and manual fan, thermal, UI, and event-log observations.
6. Execute the matching restore/off payload exactly once using the same hypothesis length.
7. Read `FanMaxGet` and raw `FanGetLevel` again; require a final `FanMaxGet=false` baseline match.
8. Append and seal the final experiment record with timings, observations, stop/recovery details, and a pass/fail/unknown outcome.

The future implementation must not use SetFanMode, SetFanLevel, `0x37`, EC access, background control, UI controls, a second payload shape, or automatic retries.

## Stop And Restore Rules

Abort before enable for invalid or duplicate flags, missing elevation/AC/approval, identity mismatch, baseline failure, non-false max-fan baseline, unavailable thermal observation, power transition, protection fault, or missing recovery route. After enable, stop immediately for timeout, exception, ambiguous readback, unexpected fan behavior, unsafe thermal trend, UI loss, crash/event-log evidence, suspend/shutdown/unplug, or any failed gate.

Restore is mandatory after an enable attempt. It uses the matching off payload exactly once, followed by immediate `FanMaxGet` and raw FanGetLevel readback. A failed or ambiguous restore is `unknown` or `fail`, retains NO-GO, records manual recovery steps, and prohibits retries or alternate-payload attempts.

## Required Log Fields

The append-only record must contain timestamp; operator and approval reference; model, SKU, BIOS, thermal policy; flags and elevation; AC/battery and thermal observation source; hypothesis length and bytes; command/type/class/method metadata; baseline, post-enable, and post-restore probe results; invocation return/error metadata; timings; manual fan/thermal/UI/event-log observations; stop reason; recovery actions; `WriteExecuted`; and final outcome. It must summarize decoded values only and never dump raw binary buffers.

## Outcome Rules

- `Pass`: every gate held, one enable attempt was observed within reviewed limits, matching restore completed, final `FanMaxGet=false` matched the baseline, and no stop condition occurred.
- `Fail`: an explicit error, safety threshold breach, failed restore, or unrecoverable state occurred.
- `Unknown`: default for missing, conflicting, incomplete, or ambiguous evidence, including any uncertain restore/readback.

A pass is evidence for independent review only. It does not select a payload, update `DeviceValidatedInputLength`, change the gate to GO, or enable normal fan control UI.

## Evidence Needed Before Input-Length Update

Only an independently reviewed exact-device record may support a later decision. It must show one length and payload, the pre-enable baseline, bounded enable result, post-enable readback, matching restore, final disabled readback, AC/battery and thermal observations, failure/recovery handling, and human approval. The first-write decision gate and proof gap checklist must then be updated together in a separately authorized design task.

## Current Implementation Boundary

The command-line runner has no UI or tray route, never retries, and never falls back between payload lengths. It creates a blocked append-only record when a command or runtime gate fails. While the application hard-codes the documented approval gates as false, it cannot reach `hpqBIOSInt0` for a write.

The [runner safety audit](set-fan-max-first-write-runner-safety-audit.md) verified these boundaries and hardened the exception path so a managed enable-transport failure still reaches the matching one-time restore attempt.

## Recommended Next Step

Obtain independent review of the baseline and the unresolved restore, thermal/power, recovery, rollback, and human-approval requirements. A separate evidence decision must change the documented gate before any execution is considered; until then, execution remains **NO-GO**.
