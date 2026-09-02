# HP Fan Control Research Abstraction Design

## Purpose

Define a narrow internal boundary for future HP fan-control research without treating it as a product control API. The boundary exists to keep explicitly approved, developer-only research operations isolated from the read-only HP Diagnostic UI.

## Current Supported Operation

The only operational research action is the developer-only four-byte SetFanMax Max Fan Pulse:

- enable payload: `01-00-00-00`
- matching restore payload: `00-00-00-00`
- command: `0x20008`, command type: `0x27`
- WMI class/method: `hpqBIntM` / `hpqBIOSInt0`

It is command-line-only, separately approval-gated, exact-device-gated, elevation-gated, AC-gated, baseline-gated, single-attempt, matching-restore, and append-only logged. It is not exposed through Settings, the tray, or the Diagnostic dashboard.

## Explicitly Unsupported Operations

This abstraction must not grow into normal fan control. The following remain unsupported and blocked:

- normal or user-facing fan control, fan curves, sliders, and toggles
- SetFanMode, SetFanLevel, and `0x37`
- EC writes, performance control, and power-limit writes
- background, scheduled, automatic, retrying, or fallback fan actions

## Internal Contracts

The pure, unwired `HpFanResearchContracts` layer now records these narrow roles without changing runner behavior:

- `IHpFanResearchOperation`: one named, bounded research operation; it has no generic speed, curve, or mode input.
- `HpFanResearchRequest`: immutable operation identity, explicitly approved payload hypothesis, and manual-observation metadata. It must never represent a UI command or persisted user preference.
- `IHpFanResearchGateEvaluator`: returns a fail-closed admission decision from command-line approval, target identity, elevation, AC, baseline, and operation-specific conditions.
- `IHpFanResearchBaselineProvider`: supplies only the approved pre/post read-only snapshots used by a research operation.
- `IHpFanResearchTransport`: an internal, operation-specific transport boundary. It must accept only the exact allowed payloads for that operation and have no mode, level, curve, or arbitrary-command API.
- `IHpFanResearchLogSink`: appends an immutable research record under `%APPDATA%\VictusX\Logs\FanExperiments\`.
- `IHpFanResearchOutcomeClassifier`: separates command return, manually observed physical response, observed restore, and readback reliability without declaring product readiness.

`HpFanResearchOperationKind` has only `FourByteMaxFanPulse`. The associated operation descriptor keeps `DeviceValidatedInputLength` null. `IHpFanResearchOperation` and `IHpFanMaxPulseResearchOperation` describe fixed pulse metadata only; neither exposes execution or generic fan-control methods. The pulse parser now carries that operation and the existing runner accepts it through a behavior-preserving pulse-specific overload.

Existing `HpFanMaxExperimentRunner`, `HpFanMaxExperimentWmiTransport`, baseline provider, log writer, and outcome classifier already map to these responsibilities. The pulse-specific contract wiring changes no flags, gates, bytes, retries, fallback behavior, restore path, log location, UI, or tray route. Keep any future refactor internal and behavior-preserving; do not create a broad `IFanControlService`.

## Required Safety Gates

Every future research operation must remain opt-in and fail closed:

- explicit developer command, acknowledgement, and operation-specific human approval
- exact HP Victus target identity, currently SKU `7Z5Z2EA#AB8`, BIOS `F.31`, and ThermalPolicyVersion `1`
- elevation and confirmed AC power
- successful approved read-only baseline, including `FanGetCount=2` and baseline `FanMaxGet=false` where applicable
- one declared payload hypothesis, one enable attempt, no retry, and no alternate-payload fallback
- matching restore in a `finally` path after an enable attempt
- readback and append-only logging even when the result is unknown or failed

## Logging And Diagnostic Boundary

Research logs must record the gate decision, exact payload bytes, baseline, enable/restore attempts, readbacks, manual observations, classifier result, blocked reasons, and whether a write was actually attempted. Logs are evidence, not authorization.

The HP Diagnostic dashboard may read the latest valid local log for status/history only. It must remain local-file-only, never invoke WMI, never refresh hardware, and expose no pulse/run button, fan toggle, slider, tray route, or other control route.

## Remaining Evidence Limits

`DeviceValidatedInputLength` remains unset because repeated physical response does not prove that one input shape is the durable, supported ABI for normal control. The four-byte pulse is preferred only for bounded developer research; one byte remains comparison-only.

`FanMaxGet` remains inconclusive: it stayed `false` during observed physical fan response. It cannot be the sole success criterion or a normal-control state contract. Raw `FanGetLevel` values remain observational only.

## Safe Extension Rules

A future research operation needs its own written protocol, exact-device evidence, dedicated approval flag, bounded payload set, baseline/readback plan, restore proof, abort/recovery plan, append-only logging, and pure gate/classifier tests. It must not inherit permission from the pulse path merely because it targets fans.

Before any normal fan-control UI can be considered, the project needs a separately reviewed product decision selecting a validated input length; durable restore semantics; a reliable success/state criterion independent of FanMaxGet alone; repeated thermal, AC/battery, failure, cancellation, and recovery evidence; and a user-facing safety design. Until then, normal fan control remains **NO-GO**.

## Recommended Next Implementation Step

If a behavior-preserving refactor is later authorized, adapt the existing developer-pulse gate, baseline, transport, logging, and classification code to these contracts with pure tests. Do not expose them to UI, configuration, tray, or background services.
