namespace GHelper.Hardware.Hp;

public enum HpFanWriteAbortReason
{
    BlockedByDefault,
    CommandNotSetFanMax,
    MissingRequiredRuntimeFlag,
    AdministratorRequired,
    PreWriteReadbackRequired,
    CurrentMaxFanStateUnknown,
    BaselineMaxFanMustBeDisabled,
    WriteTargetStateRequired,
    PostWriteReadbackRequired,
    RestorePlanRequired,
    UnexpectedExecutionRequest
}

public sealed record HpFanWritePreflightResult
{
    public bool IsAllowed { get; init; } = false;
    public IReadOnlyList<HpFanWriteAbortReason> AbortReasons { get; init; } =
    [HpFanWriteAbortReason.BlockedByDefault];
}
