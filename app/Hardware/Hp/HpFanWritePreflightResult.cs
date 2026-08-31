namespace GHelper.Hardware.Hp;

public enum HpFanWriteAbortReason
{
    BlockedByDefault,
    MissingRequiredRuntimeFlag,
    AdministratorRequired,
    PreWriteReadbackRequired,
    WriteTargetStateRequired,
    PostWriteReadbackRequired,
    VerifiedRestoreRequired,
    UnexpectedExecutionRequest
}

public sealed record HpFanWritePreflightResult
{
    public bool IsAllowed { get; init; } = false;
    public IReadOnlyList<HpFanWriteAbortReason> AbortReasons { get; init; } =
    [HpFanWriteAbortReason.BlockedByDefault];
}
