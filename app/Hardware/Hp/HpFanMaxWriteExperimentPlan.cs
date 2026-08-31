namespace GHelper.Hardware.Hp;

public enum HpFanMaxTargetState
{
    NotSpecified,
    Enabled,
    Disabled
}

public sealed record HpFanMaxWriteExperimentPlan
{
    public const string CommandName = "SetFanMax";
    public const uint CommandId = 0x27;

    public HpFanMaxTargetState TargetState { get; init; } = HpFanMaxTargetState.NotSpecified;
    public bool RequiresReadbackBeforeWrite { get; init; } = true;
    public bool RequiresReadbackAfterWrite { get; init; } = true;
    public bool RequiresVerifiedRestore { get; init; } = true;
    public bool IsWriteExecutionAllowed { get; init; } = false;
}
