using System.Text;

namespace GHelper.Hardware.Hp;

public static class HpDiagnosticReportExporter
{
    public static string ExportDirectory => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VictusX",
        "Logs",
        "Reports");

    public static string Export(string summary) => Export(summary, DateTimeOffset.Now, ExportDirectory);

    public static string Export(string summary, DateTimeOffset timestamp, string outputDirectory)
    {
        Directory.CreateDirectory(outputDirectory);
        string fileName = "hp-diagnostic-" + timestamp.ToString("yyyyMMdd-HHmmss") + ".md";
        string filePath = Path.Combine(outputDirectory, fileName);
        File.WriteAllText(filePath, BuildMarkdown(summary, timestamp), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
        return filePath;
    }

    public static string BuildMarkdown(string summary, DateTimeOffset timestamp)
    {
        return string.Join(Environment.NewLine,
            "# VictusX Read-only Diagnostic Export",
            string.Empty,
            "Exported: " + timestamp.ToString("O"),
            string.Empty,
            "> This export contains cached diagnostic data only. It does not invoke WMI or refresh hardware.",
            "> Fan control and fan writes are not implemented. SetFanMax remains NO-GO / design-only.",
            string.Empty,
            "## Diagnostic Summary",
            string.Empty,
            summary.Trim(),
            string.Empty);
    }
}
