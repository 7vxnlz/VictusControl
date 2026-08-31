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
    public void DefaultFanMaxPlan_HasNoTargetAndCannotExecute()
    {
        HpFanMaxWriteExperimentPlan plan = new();

        Assert.Equal("SetFanMax", HpFanMaxWriteExperimentPlan.CommandName);
        Assert.Equal(0x27u, HpFanMaxWriteExperimentPlan.CommandId);
        Assert.Equal(HpFanMaxTargetState.NotSpecified, plan.TargetState);
        Assert.True(plan.RequiresReadbackBeforeWrite);
        Assert.True(plan.RequiresReadbackAfterWrite);
        Assert.True(plan.RequiresVerifiedRestore);
        Assert.False(plan.IsWriteExecutionAllowed);
    }
}
