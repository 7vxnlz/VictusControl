namespace GHelper.Hardware.Hp;

public enum HpDiagnosticDashboardStatus
{
    Normal,
    Ready,
    Warning,
    Blocked
}

public sealed record HpDiagnosticDashboardRow(
    string Label,
    string Value,
    HpDiagnosticDashboardStatus Status);

public sealed record HpDiagnosticDashboardSection(
    string Title,
    IReadOnlyList<HpDiagnosticDashboardRow> Rows);

public sealed record HpDiagnosticDashboardInput
{
    public bool? IsHpVictusDetected { get; init; }
    public string? Manufacturer { get; init; }
    public string? Model { get; init; }
    public string? Sku { get; init; }
    public string? BiosVersion { get; init; }
    public string? RootWmiReadiness { get; init; }
    public string? HpqBIntMReadiness { get; init; }
    public string? HpqBDataInReadiness { get; init; }
    public string? CimRootWmiReadiness { get; init; }
    public string? CimHpqBIntMReadiness { get; init; }
    public string? CimMethodMetadataReadiness { get; init; }
    public string? SystemDesignDataDecodeStatus { get; init; }
    public string? SoftwareFanControlSupport { get; init; }
    public string? FanCount { get; init; }
    public string? MaxFanState { get; init; }
    public string? Fan1RawLevel { get; init; }
    public string? Fan2RawLevel { get; init; }
    public string? SetFanMaxWriteImplemented { get; init; }
    public string? SetFanMaxWriteAllowed { get; init; }
    public string? SetFanMaxBlockedReason { get; init; }
    public string? SetFanMaxNextRequiredProof { get; init; }
}
