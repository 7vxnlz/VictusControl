namespace GHelper.Hardware.Hp;

public static class HpFanMaxExperimentRunLogMapper
{
    public static HpFanMaxExperimentLogRecord Create(HpFanMaxExperimentRunResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        HpFanMaxExperimentBaseline? baseline = result.Baseline;
        bool writeAttempted = result.EnableWrite.Attempted || result.RestoreWrite.Attempted;
        return new HpFanMaxExperimentLogRecord
        {
            PayloadLengthCandidate = result.Payload?.Candidate,
            PayloadBytesHypothesis = result.Payload?.EnableBytesHex,
            Model = baseline?.Model,
            Sku = baseline?.Sku,
            BiosVersion = baseline?.BiosVersion,
            ThermalPolicyVersion = baseline?.ThermalPolicyVersion,
            BaselineFanGetCount = baseline?.FanGetCount,
            BaselineFanMaxGet = baseline?.FanMaxGetEnabled,
            BaselineFanGetLevelRaw = baseline?.FanGetLevelRaw,
            BaselineCapturePerformed = result.BaselineCapturePerformed,
            BaselineCaptureResult = result.BaselineCapturePerformed ? "Approved read-only baseline capture completed." : "Baseline capture was blocked before WMI or hardware action.",
            BaselineReadOnlyProbeSummary = baseline?.ReadOnlyProbeSummary ?? [],
            EnableResult = FormatResult(result.EnableWrite),
            PostEnableFanMaxGet = result.PostEnableReadback?.FanMaxGetEnabled,
            PostEnableFanGetLevelRaw = result.PostEnableReadback?.FanGetLevelRaw,
            RestoreResult = FormatResult(result.RestoreWrite),
            PostRestoreFanMaxGet = result.PostRestoreReadback?.FanMaxGetEnabled,
            PostRestoreFanGetLevelRaw = result.PostRestoreReadback?.FanGetLevelRaw,
            ManualObservationNotes = "No manual observations are collected automatically. DeviceValidatedInputLength remains unset.",
            Outcome = result.Outcome,
            BlockedReasons = result.BlockedReasons,
            WriteExecuted = writeAttempted
        };
    }

    private static string FormatResult(HpFanMaxExperimentWriteResult result) =>
        result.Attempted
            ? result.Succeeded ? "Attempted and succeeded." : "Attempted and failed: " + (result.Error ?? "unknown error")
            : "Not attempted: " + (result.Error ?? "blocked");
}
