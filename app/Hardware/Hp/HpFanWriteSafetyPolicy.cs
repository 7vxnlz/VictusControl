namespace GHelper.Hardware.Hp;

public sealed record HpFanWriteSafetyPolicy
{
    public static readonly IReadOnlyList<string> DefaultRequiredFlags =
    [
        "--hp-victus",
        "--hp-fan-write-experiment",
        "--hp-wmi-write-manual-test",
        "--hp-fan-write-acknowledge-risk"
    ];

    public IReadOnlyList<string> RequiredFlags { get; init; } = DefaultRequiredFlags;
    public bool RequiresAdministrator { get; init; } = true;
    public bool RequiresReadbackBeforeWrite { get; init; } = true;
    public bool RequiresReadbackAfterWrite { get; init; } = true;
    public bool RequiresVerifiedRestore { get; init; } = true;
    public bool IsWriteExecutionAllowed { get; init; } = false;
}
