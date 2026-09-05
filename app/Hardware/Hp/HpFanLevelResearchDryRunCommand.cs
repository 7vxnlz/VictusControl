using System.Globalization;
using System.Text.Json;

namespace GHelper.Hardware.Hp;

// Descriptive research data only; this is deliberately not a transport command or operation contract.
public sealed record HpFanLevelResearchDryRunRecord(
    byte? RawLevelCandidate,
    IReadOnlyList<string> ValidationReasons)
{
    public int SchemaVersion => 1;
    public DateTimeOffset TimestampUtc { get; } = DateTimeOffset.UtcNow;
    public string Operation => "SetFanLevelResearchDryRun";
    public bool IsValidCandidate => RawLevelCandidate.HasValue && ValidationReasons.Count == 0;
    public string Status => "Not executable / not validated";
    public string WmiNamespaceCandidate => @"root\wmi";
    public string WmiClassCandidate => "hpqBIntM";
    public string WmiMethodCandidate => "hpqBIOSInt0";
    public string CommandCandidate => "0x20008";
    public string CommandTypeCandidate => "0x2E";
    public string PayloadShapeHypothesis => "Two equal raw bytes; fan mapping and input ABI unvalidated";
    public string? PayloadHexCandidate => IsValidCandidate ? $"{RawLevelCandidate:X2}-{RawLevelCandidate:X2}" : null;
    public int? CandidateInputLength => IsValidCandidate ? 2 : null;
    public string LevelUnits => "Unknown; raw byte only, not RPM or percent";
    public bool IsExecutable => false;
    public bool WriteExecuted => false;
    public bool WmiInvoked => false;
    public bool NoHardwareInvocation => true;
    public bool NoWmiInvocation => true;
    public bool FirstWriteGateSatisfied => false;
    public int? DeviceValidatedInputLength => null;
    public bool NormalControlValidated => false;
    public bool UserFacingControlAllowed => false;
    public string NormalFanControlDecision => "NO-GO";
    public string SafetyNote => "Developer-only serialization. The 0-255 bound is byte representation only, not a safe hardware range. No restore semantics are validated. No hardware action is available.";

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
}

public sealed record HpFanLevelResearchDryRunCommandResult(
    bool ShouldExit,
    HpFanLevelResearchDryRunRecord? Record)
{
    public bool IsValidRequest => Record?.IsValidCandidate == true;
}

public static class HpFanLevelResearchDryRunCommand
{
    public const string DryRunFlag = "--hp-fan-level-research-dry-run";
    public const string HpVictusFlag = "--hp-victus";
    public const string LevelPrefix = "--fan-level-candidate=";
    private const string LevelFlag = "--fan-level-candidate";

    public static HpFanLevelResearchDryRunCommandResult Parse(IEnumerable<string> arguments)
    {
        ArgumentNullException.ThrowIfNull(arguments);
        string[] args = arguments.ToArray();

        // Consume malformed/mixed research requests before any hardware-capable startup route.
        if (!args.Any(arg => IsFlagOrAssignment(arg, DryRunFlag) || IsFlagOrAssignment(arg, LevelFlag)))
        {
            return new(false, null);
        }

        List<string> reasons = [];
        if (args.Count(arg => EqualsFlag(arg, DryRunFlag)) != 1)
        {
            reasons.Add("Exactly one --hp-fan-level-research-dry-run flag is required.");
        }

        if (args.Count(arg => EqualsFlag(arg, HpVictusFlag)) != 1)
        {
            reasons.Add("Exactly one --hp-victus flag is required.");
        }

        if (args.Any(arg => !EqualsFlag(arg, DryRunFlag) && !EqualsFlag(arg, HpVictusFlag)
            && !arg.StartsWith(LevelPrefix, StringComparison.OrdinalIgnoreCase)))
        {
            reasons.Add("Only HP mode, the dry-run flag, and one candidate are permitted; probes, experiments, approvals, and other arguments are rejected.");
        }

        string[] levels = args.Where(arg => arg.StartsWith(LevelPrefix, StringComparison.OrdinalIgnoreCase)).ToArray();
        byte? candidate = null;
        if (levels.Length == 1 && byte.TryParse(levels[0][LevelPrefix.Length..], NumberStyles.None,
            CultureInfo.InvariantCulture, out byte rawLevel))
        {
            candidate = rawLevel;
        }
        else
        {
            reasons.Add("Exactly one --fan-level-candidate=<integer 0-255> is required; this is a raw byte bound, not a validated fan range.");
        }

        return new(true, new(reasons.Count == 0 ? candidate : null, reasons.ToArray()));
    }

    private static bool EqualsFlag(string argument, string flag) =>
        string.Equals(argument, flag, StringComparison.OrdinalIgnoreCase);

    private static bool IsFlagOrAssignment(string argument, string flag) =>
        EqualsFlag(argument, flag) || argument.StartsWith(flag + "=", StringComparison.OrdinalIgnoreCase);
}
