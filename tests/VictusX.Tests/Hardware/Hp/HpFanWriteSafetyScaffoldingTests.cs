using GHelper.Hardware.Hp;
using Xunit;

namespace VictusX.Tests.Hardware.Hp;

public sealed class HpFanWriteSafetyScaffoldingTests
{
    [Fact]
    public void DefaultPolicy_IsBlockedAndRequiresEverySafetyGate()
    {
        HpFanWriteSafetyPolicy policy = new();

        Assert.False(policy.IsWriteExecutionAllowed);
        Assert.True(policy.RequiresAdministrator);
        Assert.True(policy.RequiresReadbackBeforeWrite);
        Assert.True(policy.RequiresReadbackAfterWrite);
        Assert.True(policy.RequiresVerifiedRestore);
        Assert.Equal(
            [
                "--hp-victus",
                "--hp-fan-write-experiment",
                "--hp-wmi-write-manual-test",
                "--hp-fan-write-acknowledge-risk"
            ],
            policy.RequiredFlags);
    }

    [Fact]
    public void DefaultPreflight_IsNotAllowedAndExplainsWhy()
    {
        HpFanWritePreflightResult result = new();

        Assert.False(result.IsAllowed);
        Assert.Equal([HpFanWriteAbortReason.BlockedByDefault], result.AbortReasons);
    }

    [Fact]
    public void DefaultFanMaxPlan_HasNoTargetOrRestoreAndCannotExecute()
    {
        HpFanMaxWriteExperimentPlan plan = new();

        Assert.Equal("SetFanMax", HpFanMaxWriteExperimentPlan.CommandName);
        Assert.Equal(0x27u, HpFanMaxWriteExperimentPlan.CommandId);
        Assert.Null(plan.TargetState);
        Assert.Null(plan.RestoreTargetState);
        Assert.True(plan.RequiresReadbackBeforeWrite);
        Assert.True(plan.RequiresReadbackAfterWrite);
        Assert.True(plan.RequiresVerifiedRestore);
        Assert.False(plan.IsWriteExecutionAllowed);
    }

    [Theory]
    [InlineData(HpFanMaxTargetState.EnableMaxFan, 1)]
    [InlineData(HpFanMaxTargetState.RestoreDisableMaxFan, 0)]
    public void PayloadDescription_DescribesOnlyTheTwoApprovedFutureStates(
        HpFanMaxTargetState targetState,
        byte expectedFirstByteValue)
    {
        HpFanMaxPayloadDescription description = HpFanMaxPayloadDescription.Describe(targetState);

        Assert.Equal(targetState, description.TargetState);
        Assert.Equal(expectedFirstByteValue, description.FirstByteValue);
        Assert.Equal("hpqBIOSInt0", HpFanMaxPayloadDescription.ReferenceMethodName);
        Assert.Equal(0x20008u, HpFanMaxPayloadDescription.ReferenceCommandValue);
        Assert.Equal(0x27u, HpFanMaxPayloadDescription.ReferenceCommandType);
        Assert.Equal(0, HpFanMaxPayloadDescription.ReferenceExpectedOutputSize);
        Assert.Null(description.DeviceValidatedInputLength);
        Assert.Collection(
            description.ObservedReferenceInputShapes,
            fourByteShape =>
            {
                Assert.Equal(4, fourByteShape.InputLength);
                Assert.Equal(3, fourByteShape.ZeroFilledTrailingByteCount);
            },
            oneByteShape =>
            {
                Assert.Equal(1, oneByteShape.InputLength);
                Assert.Equal(0, oneByteShape.ZeroFilledTrailingByteCount);
            });
    }

    [Fact]
    public void Preflight_DefaultRequest_IsBlockedByEveryMissingRequirement()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(new());

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.CommandNotSetFanMax, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.MissingRequiredRuntimeFlag, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.AdministratorRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.InteractiveHumanConfirmationRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.ApprovedDeviceBaselineRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.HealthyReadOnlyBaselineRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.StableAcPowerRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.IndependentThermalObservationRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.PreWriteReadbackRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.CurrentMaxFanStateUnknown, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.WriteTargetStateRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.PostWriteReadbackRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.RestorePlanRequired, result.AbortReasons);
        Assert.Contains(HpFanWriteAbortReason.SingleWriteAttemptRequired, result.AbortReasons);
    }

    [Fact]
    public void Preflight_NonSetFanMaxCommand_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            RequestedCommandName = "SetFanLevel",
            RequestedCommandId = 0x2E
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.CommandNotSetFanMax, result.AbortReasons);
    }

    [Fact]
    public void Preflight_MissingPreRead_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            HasSuccessfulPreReadFanMaxGet = false
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.PreWriteReadbackRequired, result.AbortReasons);
    }

    [Fact]
    public void Preflight_UnknownCurrentState_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            CurrentMaxFanEnabled = null
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.CurrentMaxFanStateUnknown, result.AbortReasons);
    }

    [Fact]
    public void Preflight_MissingRestorePlan_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            Plan = new HpFanMaxWriteExperimentPlan
            {
                TargetState = HpFanMaxTargetState.EnableMaxFan
            }
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.RestorePlanRequired, result.AbortReasons);
    }

    [Fact]
    public void Preflight_NonAdmin_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            IsAdministrator = false
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.AdministratorRequired, result.AbortReasons);
    }

    [Fact]
    public void Preflight_MissingExplicitWriteFlag_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            PresentFlags = ["--hp-victus"]
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.MissingRequiredRuntimeFlag, result.AbortReasons);
    }

    [Fact]
    public void Preflight_RestoreStateCannotBeTheInitialWrite()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            Plan = new HpFanMaxWriteExperimentPlan
            {
                TargetState = HpFanMaxTargetState.RestoreDisableMaxFan,
                RestoreTargetState = HpFanMaxTargetState.RestoreDisableMaxFan
            }
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.InitialWriteMustEnableMaxFan, result.AbortReasons);
    }

    [Fact]
    public void Preflight_MissingHumanConfirmation_IsBlocked()
    {
        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(ApprovedRequest() with
        {
            HasInteractiveHumanConfirmation = false
        });

        Assert.False(result.IsAllowed);
        Assert.Contains(HpFanWriteAbortReason.InteractiveHumanConfirmationRequired, result.AbortReasons);
    }

    [Fact]
    public void Preflight_TheoreticalFullyApprovedRequest_IsAllowedButCannotExecute()
    {
        HpFanMaxWritePreflightRequest request = ApprovedRequest();

        HpFanWritePreflightResult result = HpFanMaxWritePreflightEvaluator.Evaluate(request);

        Assert.True(result.IsAllowed);
        Assert.Empty(result.AbortReasons);
        Assert.False(request.Plan.IsWriteExecutionAllowed);
    }

    private static HpFanMaxWritePreflightRequest ApprovedRequest() => new()
    {
        RequestedCommandName = HpFanMaxWriteExperimentPlan.CommandName,
        RequestedCommandId = HpFanMaxWriteExperimentPlan.CommandId,
        PresentFlags = HpFanWriteSafetyPolicy.DefaultRequiredFlags,
        IsAdministrator = true,
        HasInteractiveHumanConfirmation = true,
        HasApprovedDeviceBaseline = true,
        HasHealthyReadOnlyBaseline = true,
        HasStableAcPower = true,
        HasIndependentThermalObservation = true,
        HasSuccessfulPreReadFanMaxGet = true,
        CurrentMaxFanEnabled = false,
        HasPostWriteReadbackPlan = true,
        HasRestoreReadbackPlan = true,
        IsSingleWriteAttempt = true,
        Plan = new HpFanMaxWriteExperimentPlan
        {
            TargetState = HpFanMaxTargetState.EnableMaxFan,
            RestoreTargetState = HpFanMaxTargetState.RestoreDisableMaxFan
        }
    };
}
