namespace GHelper.Hardware.Hp;

public sealed record HpFanMaxWritePreflightRequest
{
    public string RequestedCommandName { get; init; } = string.Empty;
    public uint? RequestedCommandId { get; init; }
    public IReadOnlyList<string> PresentFlags { get; init; } = [];
    public bool IsAdministrator { get; init; } = false;
    public bool HasSuccessfulPreReadFanMaxGet { get; init; } = false;
    public bool? CurrentMaxFanEnabled { get; init; }
    public bool HasPostWriteReadbackPlan { get; init; } = false;
    public HpFanMaxWriteExperimentPlan Plan { get; init; } = new();
}
