namespace GHelper.Hardware.Hp;

public static class HpDiagnosticStatusText
{
    public const string Title = "VictusX Read-only Diagnostic";
    public const string ReadOnlyDiagnostic = "Read-only diagnostic";
    public const string ReportNotAvailable = "Report not available";
    public const string ReportCouldNotBeRead = "Report could not be read";
    public const string SomeFieldsNotAvailable = "Some fields are not available";
    public const string FanControlNotImplemented = "Fan control is not implemented";
    public const string SetFanMaxNoGo = "SetFanMax is NO-GO / design-only";

    public static string SafetyWarning => "READ-ONLY: " + FanControlNotImplemented + ". " + SetFanMaxNoGo + ".";
}
