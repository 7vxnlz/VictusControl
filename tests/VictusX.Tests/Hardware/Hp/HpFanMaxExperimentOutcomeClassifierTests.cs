using GHelper.Hardware.Hp;
using Xunit;

namespace VictusX.Tests.Hardware.Hp;

public sealed class HpFanMaxExperimentOutcomeClassifierTests
{
    [Fact]
    public void SuccessfulCommandWithPhysicalResponseAndFalseFanMaxGet_IsReadbackInconclusive()
    {
        HpFanMaxExperimentLogRecord record = CreateWrittenRecord() with
        {
            EnableCommandSucceeded = true,
            RestoreCommandSucceeded = true,
            FanMaxGetConfirmedEnable = false,
            PhysicalFanResponseObserved = true,
            RestoreObserved = true,
            ReadbackReliability = HpFanMaxExperimentReadbackReliability.Inconclusive
        };

        Assert.Equal(
            HpFanMaxExperimentalOutcomeClassification.CommandSucceededPhysicalResponseObservedReadbackInconclusive,
            record.ExperimentalOutcomeClassification);
        Assert.Null(record.DeviceValidatedInputLength);
    }

    [Fact]
    public void SuccessfulCommandWithoutPhysicalConfirmation_RemainsFailOrUnknownRatherThanValidated()
    {
        HpFanMaxExperimentLogRecord record = CreateWrittenRecord() with
        {
            EnableCommandSucceeded = true,
            RestoreCommandSucceeded = true,
            PhysicalFanResponseObserved = false,
            Outcome = HpFanMaxExperimentOutcome.Fail
        };

        Assert.Equal(HpFanMaxExperimentalOutcomeClassification.CommandSucceededNoPhysicalConfirmation, record.ExperimentalOutcomeClassification);
        Assert.Equal(HpFanMaxExperimentOutcome.Fail, record.Outcome);
        Assert.Null(record.DeviceValidatedInputLength);
    }

    [Fact]
    public void RestoreFailure_IsClassifiedAsRestoreFailed()
    {
        HpFanMaxExperimentLogRecord record = CreateWrittenRecord() with
        {
            EnableCommandSucceeded = true,
            RestoreCommandSucceeded = false,
            PhysicalFanResponseObserved = true,
            Outcome = HpFanMaxExperimentOutcome.Fail
        };

        Assert.Equal(HpFanMaxExperimentalOutcomeClassification.RestoreFailed, record.ExperimentalOutcomeClassification);
        Assert.Null(record.DeviceValidatedInputLength);
    }

    [Fact]
    public void UnsafeAbort_IsClassifiedWithoutGrantingAnyApproval()
    {
        HpFanMaxExperimentLogRecord record = CreateWrittenRecord() with { UnsafeAbortObserved = true };

        Assert.Equal(HpFanMaxExperimentalOutcomeClassification.UnsafeAbort, record.ExperimentalOutcomeClassification);
        Assert.False(record.FirstWriteGateSatisfied);
        Assert.Null(record.DeviceValidatedInputLength);
    }

    private static HpFanMaxExperimentLogRecord CreateWrittenRecord() => new()
    {
        WriteExecuted = true,
        PayloadLengthCandidate = HpFanMaxExperimentPayloadLengthCandidate.FourByteHypothesis,
        PayloadBytesHypothesis = "01-00-00-00"
    };
}
