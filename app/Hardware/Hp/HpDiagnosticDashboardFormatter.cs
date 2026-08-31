namespace GHelper.Hardware.Hp;

public static class HpDiagnosticDashboardFormatter
{
    public const string NotAvailable = "Not available";
    public const string RawFanLevelWarning = "Raw values are not RPM or percent.";
    public const string FanControlStatus = "Blocked - not implemented";
    public const string SetFanMaxStatus = "NO-GO / design-only";

    public static IReadOnlyList<HpDiagnosticDashboardSection> BuildSections(HpDiagnosticDashboardInput input)
    {
        return
        [
            new("Device",
            [
                Row("Read-only diagnostic", "Cached report and startup snapshot only; no UI action invokes WMI."),
                Row("HP/Victus detected", input.IsHpVictusDetected == true ? "Detected" : NotAvailable),
                Row("Manufacturer", input.Manufacturer),
                Row("Model", input.Model),
                Row("SKU", input.Sku),
                Row("BIOS version", input.BiosVersion)
            ]),
            new("WMI/CIM readiness",
            [
                Row(@"root\wmi", input.RootWmiReadiness),
                Row("hpqBIntM", input.HpqBIntMReadiness),
                Row("hpqBDataIn", input.HpqBDataInReadiness),
                Row(@"CIM root\wmi", input.CimRootWmiReadiness),
                Row("CIM hpqBIntM", input.CimHpqBIntMReadiness),
                Row("CIM method metadata", input.CimMethodMetadataReadiness)
            ]),
            new("Read-only telemetry",
            [
                Row("SystemDesignData decoded", input.SystemDesignDataDecodeStatus),
                Row("Software fan control declared by firmware", input.SoftwareFanControlSupport)
            ]),
            new("Fan read-only status",
            [
                Row("Fan count", input.FanCount),
                Row("Max fan state", input.MaxFanState),
                Row("Fan 1 raw level byte", input.Fan1RawLevel),
                Row("Fan 2 raw level byte", input.Fan2RawLevel),
                Row("Raw level data", RawFanLevelWarning)
            ]),
            new("Safety / NO-GO status",
            [
                Row("Fan control", FanControlStatus),
                Row("SetFanMax", SetFanMaxStatus),
                Row("SetFanMax write implemented", input.SetFanMaxWriteImplemented),
                Row("SetFanMax write allowed", input.SetFanMaxWriteAllowed),
                Row("Blocked reason", input.SetFanMaxBlockedReason),
                Row("Next required proof", input.SetFanMaxNextRequiredProof)
            ])
        ];
    }

    public static string BuildSummary(HpDiagnosticDashboardInput input)
    {
        return string.Join(
            Environment.NewLine,
            BuildSections(input)
                .SelectMany(section => section.Rows)
                .Select(row => row.Label + ": " + row.Value));
    }

    public static string FormatCimReadiness(bool? value) => value switch
    {
        true => "Ready",
        false => NotAvailable,
        _ => NotAvailable
    };

    public static string FormatWriteImplementationStatus(bool? value) => value switch
    {
        true => "Implemented",
        false => "Not implemented",
        _ => NotAvailable
    };

    public static string FormatWriteAllowedStatus(bool? value) => value switch
    {
        true => "Allowed",
        false => "Blocked",
        _ => NotAvailable
    };

    private static HpDiagnosticDashboardRow Row(string label, string? value)
    {
        string displayValue = string.IsNullOrWhiteSpace(value) ? NotAvailable : value;
        return new HpDiagnosticDashboardRow(label, displayValue, GetStatus(displayValue));
    }

    private static HpDiagnosticDashboardStatus GetStatus(string value)
    {
        if (value.StartsWith("Ready", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("Succeeded", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("Enabled", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("Declared", StringComparison.OrdinalIgnoreCase))
        {
            return HpDiagnosticDashboardStatus.Ready;
        }

        if (value.StartsWith("Blocked", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("NO-GO", StringComparison.OrdinalIgnoreCase))
        {
            return HpDiagnosticDashboardStatus.Blocked;
        }

        return HpDiagnosticDashboardStatus.Normal;
    }
}
