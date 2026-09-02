using Xunit;

namespace VictusX.Tests.Hardware.Hp;

public sealed class HpDiagnosticPreviewConfigurationTests
{
    [Fact]
    public void Launcher_UsesOnlyTheSafeHpVictusArgument()
    {
        string launcher = ReadRepositoryFile("tools", "run-victusx-hp-diagnostic.ps1");

        Assert.Contains("-ArgumentList \"--hp-victus\"", launcher, StringComparison.Ordinal);
        Assert.DoesNotContain("--hp-wmi-readonly-test", launcher, StringComparison.Ordinal);
    }

    [Fact]
    public void PublishProfile_DoesNotContainDeveloperOnlyWmiTestArgument()
    {
        string profile = ReadRepositoryFile(
            "app",
            "Properties",
            "PublishProfiles",
            "VictusX-HP-Diagnostic-win-x64.pubxml");

        Assert.Contains("<SelfContained>true</SelfContained>", profile, StringComparison.Ordinal);
        Assert.DoesNotContain("--hp-wmi-readonly-test", profile, StringComparison.Ordinal);
    }

    [Fact]
    public void PublishProfile_OptsOutOfTheInheritedZipOnPublishTarget()
    {
        string profile = ReadRepositoryFile(
            "app",
            "Properties",
            "PublishProfiles",
            "VictusX-HP-Diagnostic-win-x64.pubxml");
        string project = ReadRepositoryFile("app", "VictusX.csproj");

        Assert.Contains("<SkipLegacySingleExeZip>true</SkipLegacySingleExeZip>", profile, StringComparison.Ordinal);
        Assert.Contains("'$(SkipLegacySingleExeZip)'!='true'", project, StringComparison.Ordinal);
    }

    [Fact]
    public void SettingsUi_HasNoSetFanMaxExperimentRoute()
    {
        string settings = ReadRepositoryFile("app", "Settings.cs");

        Assert.DoesNotContain("HpFanMaxExperimentRunner", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("HpFanMaxExperimentWmiTransport", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("--hp-fan-write-experiment", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("HpFanMaxPulseCommand", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("--hp-fan-max-pulse", settings, StringComparison.Ordinal);
    }

    [Fact]
    public void HpDiagnosticQuit_UsesDedicatedUiThreadShellExit()
    {
        string settings = ReadRepositoryFile("app", "Settings.cs");
        string program = ReadRepositoryFile("app", "Program.cs");

        Assert.Contains("Program.ExitHpDiagnosticShell();", settings, StringComparison.Ordinal);
        Assert.Contains("internal static void ExitHpDiagnosticShell()", program, StringComparison.Ordinal);
        Assert.Contains("Application.ExitThread();", program, StringComparison.Ordinal);
        Assert.Contains("if (!hpVictusMode)", program, StringComparison.Ordinal);
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        string repositoryRoot = FindRepositoryRoot();
        return File.ReadAllText(Path.Combine([repositoryRoot, .. segments]));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "VictusX.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the VictusX repository root.");
    }
}
