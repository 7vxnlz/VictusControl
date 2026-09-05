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
        Assert.DoesNotContain("HpFanMaxHoldCommand", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("--hp-fan-max-hold", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("HpWmiInvocationClient", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("HpFanLevelResearch", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("--hp-fan-level-research-dry-run", settings, StringComparison.Ordinal);
    }

    [Fact]
    public void FanLevelDryRun_ExitsBeforeAllHardwareStartupAndHasNoUiOrTransportDependency()
    {
        string program = ReadRepositoryFile("app", "Program.cs").Replace("\r\n", "\n");
        Assert.Contains("public static void Main(string[] args)\n        {\n            if (TryRunHpFanLevelResearchDryRun(args))\n            {\n                return;\n            }", program, StringComparison.Ordinal);
        int start = program.IndexOf("private static bool TryRunHpFanLevelResearchDryRun", StringComparison.Ordinal);
        int end = program.IndexOf("private static bool TryRunHpFanMaxHold", start, StringComparison.Ordinal);
        string route = program[start..end];
        Assert.Contains("Console.WriteLine(result.Record!.ToJson());", route, StringComparison.Ordinal);
        Assert.Contains("Environment.ExitCode = result.IsValidRequest ? 0 : 2;", route, StringComparison.Ordinal);
        Assert.True(route.IndexOf("HpFanLevelResearchDryRunLogWriter.Write", StringComparison.Ordinal) < route.IndexOf("Console.WriteLine", StringComparison.Ordinal));
        Assert.Contains("Environment.ExitCode = 1;", route, StringComparison.Ordinal);
        Assert.DoesNotContain("TryRunHpFanLevelResearchDryRun", program[end..], StringComparison.Ordinal);

        string source = ReadRepositoryFile("app", "Hardware", "Hp", "HpFanLevelResearchDryRunCommand.cs");
        source += ReadRepositoryFile("app", "Hardware", "Hp", "HpFanLevelResearchDryRunLogWriter.cs");
        string[] forbidden =
        [
            "System.Management", "Microsoft.Management", "DllImport", "LibraryImport", "Process.Start",
            "HpWmiInvocationClient", "HpFanMaxExperiment", "IHpFanResearchOperation", "HpBiosWmiCommandCatalog",
            "PawnIO", "PwnIO", "WinRing0", "EmbeddedController", "LibreHardwareMonitor", "0x37",
            "SetFanMode", "System.Windows.Forms", "Slider", "Toggle", "Button", "Task.Run", "Timer"
        ];
        foreach (string term in forbidden)
        {
            Assert.DoesNotContain(term, source + route, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void DeveloperHoldRoute_HasNoLowLevelOrGenericFanFallbackSurface()
    {
        string source = string.Join(
            Environment.NewLine,
            ReadRepositoryFile("app", "Hardware", "Hp", "HpFanMaxHoldCommand.cs"),
            ReadRepositoryFile("app", "Hardware", "Hp", "HpFanMaxExperimentRunner.cs"),
            ReadRepositoryFile("app", "Hardware", "Hp", "HpFanMaxExperimentRuntime.cs"));
        string[] forbiddenTerms =
        [
            "PawnIO",
            "PwnIO",
            "WinRing0",
            "LibreHardwareMonitor",
            "EmbeddedController",
            "SetFanMode",
            "SetFanLevel",
            "0x37"
        ];

        foreach (string forbiddenTerm in forbiddenTerms)
        {
            Assert.DoesNotContain(forbiddenTerm, source, StringComparison.OrdinalIgnoreCase);
        }
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

    [Fact]
    public void HpDiagnosticDashboard_UsesScrollableContentWithinWorkingAreaBounds()
    {
        string settings = ReadRepositoryFile("app", "Settings.cs");

        Assert.Contains("AutoScroll = true", settings, StringComparison.Ordinal);
        Assert.Contains("scrollHost.VerticalScroll.Visible = false;", settings, StringComparison.Ordinal);
        Assert.Contains("scrollHost.HorizontalScroll.Visible = false;", settings, StringComparison.Ordinal);
        Assert.Contains("Dock = DockStyle.Fill", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("ConfigureHpDiagnosticWindowBounds", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("AddHpReadOnlyMainShell", settings, StringComparison.Ordinal);
        Assert.Contains("Text = \"Diagnostic\"", settings, StringComparison.Ordinal);
        Assert.Contains("EnsureHpDiagnosticForm();", settings, StringComparison.Ordinal);
        Assert.Contains("PositionHpDiagnosticForm();", settings, StringComparison.Ordinal);
        Assert.Contains("hpDiagnosticForm?.Hide();", settings, StringComparison.Ordinal);
        Assert.Contains("HpFanMaxPulseHistoryLoader.Load", settings, StringComparison.Ordinal);
        Assert.Contains("HpFanProofGapAnalyzer.Analyze", settings, StringComparison.Ordinal);
    }

    [Fact]
    public void HpMode_UsesDisabledInheritedShellWithFooterDiagnosticAction()
    {
        string settings = ReadRepositoryFile("app", "Settings.cs");

        Assert.Contains("hpMainShellPanel = panelPerformance;", settings, StringComparison.Ordinal);
        Assert.Contains("panelPerformance,", settings, StringComparison.Ordinal);
        Assert.Contains("panelGPU,", settings, StringComparison.Ordinal);
        Assert.Contains("panelScreen,", settings, StringComparison.Ordinal);
        Assert.Contains("panelKeyboard,", settings, StringComparison.Ordinal);
        Assert.Contains("panelBattery", settings, StringComparison.Ordinal);
        Assert.Contains("section.Enabled = true;", settings, StringComparison.Ordinal);
        Assert.Contains("ConfigureHpReadOnlySection(section);", settings, StringComparison.Ordinal);
        Assert.Contains("control.ForeColor = foreMain;", settings, StringComparison.Ordinal);
        Assert.Contains("control is RButton or ComboBox or Slider or CheckBox or PictureBox", settings, StringComparison.Ordinal);
        Assert.Contains("buttonDonate.Text = AppConfig.IsHpVictusHardwareMode() ? \"Thank You\"", settings, StringComparison.Ordinal);
        Assert.Contains("buttonUpdates.Enabled = false;", settings, StringComparison.Ordinal);
        Assert.Contains("tableButtons.ColumnCount = 4;", settings, StringComparison.Ordinal);
        Assert.Contains("for (int column = 0; column < 4; column++)", settings, StringComparison.Ordinal);
        Assert.Contains("new ColumnStyle(SizeType.Percent, 25F)", settings, StringComparison.Ordinal);
        Assert.Contains("tableButtons.AutoSize = false;", settings, StringComparison.Ordinal);
        Assert.Contains("BackColor = buttonSecond,", settings, StringComparison.Ordinal);
        Assert.Contains("Image = Properties.Resources.icons8_log_32,", settings, StringComparison.Ordinal);
        Assert.Contains("hpDiagnosticFooterButton.FlatAppearance.BorderColor = borderSecond;", settings, StringComparison.Ordinal);
        Assert.Contains("hpDiagnosticFooterButton.Click += ButtonHpDiagnostic_Click;", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("ConfigureHpFooterButtonText", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("CreateHpCompactButtonImage", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("AddHpReadOnlyOverviewPanel", settings, StringComparison.Ordinal);
    }

    [Fact]
    public void HpDiagnostic_UsesOwnedSidePanelLikeUpdatesInsteadOfMainWindowReplacement()
    {
        string settings = ReadRepositoryFile("app", "Settings.cs");

        Assert.Contains("RForm? hpDiagnosticForm;", settings, StringComparison.Ordinal);
        Assert.Contains("Name = \"HpDiagnosticSidePanel\"", settings, StringComparison.Ordinal);
        Assert.Contains("ShowInTaskbar = false", settings, StringComparison.Ordinal);
        Assert.Contains("StartPosition = FormStartPosition.Manual", settings, StringComparison.Ordinal);
        Assert.Contains("AddOwnedForm(hpDiagnosticForm);", settings, StringComparison.Ordinal);
        Assert.Contains("ApplyHpDarkExplorerTheme(scrollHost);", settings, StringComparison.Ordinal);
        Assert.Contains("SetWindowTheme(control.Handle, \"DarkMode_Explorer\", null);", settings, StringComparison.Ordinal);
        Assert.Contains("int left = Left - sideWidth - 5;", settings, StringComparison.Ordinal);
        Assert.DoesNotContain("hpMainShellPanel?.Hide();", settings, StringComparison.Ordinal);
    }

    [Fact]
    public void DisabledButtons_KeepThemeSafeTextAndReserveHorizontalIconSpace()
    {
        string button = ReadRepositoryFile("app", "UI", "RButton.cs");

        Assert.Contains("TextImageRelation == TextImageRelation.ImageAboveText", button, StringComparison.Ordinal);
        Assert.Contains("TextImageRelation == TextImageRelation.ImageBeforeText", button, StringComparison.Ordinal);
        Assert.Contains("int horizontalImageReserve = Image.Width + Padding.Left + 6;", button, StringComparison.Ordinal);
        Assert.Contains("rect.X += horizontalImageReserve;", button, StringComparison.Ordinal);
        Assert.Contains("rect.Width -= horizontalImageReserve;", button, StringComparison.Ordinal);
        Assert.Contains("pevent.Graphics.FillRectangle(brush, ClientRectangle);", button, StringComparison.Ordinal);
        Assert.Contains("pevent.Graphics.DrawImage(Image, imageRect);", button, StringComparison.Ordinal);
        Assert.Contains("else if (Image is null)", button, StringComparison.Ordinal);
        Assert.Contains("Color disabledTextColor = Color.FromArgb(", button, StringComparison.Ordinal);
        Assert.Contains("imageBeforeText ? TextFormatFlags.Left : TextFormatFlags.HorizontalCenter", button, StringComparison.Ordinal);
        Assert.Contains("TextRenderer.DrawText(pevent.Graphics, Text, Font, rect, disabledTextColor, flags);", button, StringComparison.Ordinal);
        Assert.Contains("TextFormatFlags.SingleLine | TextFormatFlags.NoPadding", button, StringComparison.Ordinal);
    }

    [Fact]
    public void HpLiveTelemetry_HasOnlyOsQueriesAndCannotReachInheritedControls()
    {
        string source = ReadRepositoryFile("app", "Hardware", "Hp", "HpWindowsTelemetrySource.cs");
        string provider = ReadRepositoryFile("app", "Hardware", "Hp", "HpReadOnlyTelemetry.cs");
        foreach (string forbidden in new[] { "System.Management", "HpWmi", "hpqBIOSInt", "AsusACPI",
            "HardwareControl", "IHardwareController", "DeviceSet", "Process.Start", "HpFanMaxExperiment", "DeviceValidatedInputLength" })
        {
            Assert.DoesNotContain(forbidden, source, StringComparison.Ordinal);
            Assert.DoesNotContain(forbidden, provider, StringComparison.Ordinal);
        }
        Assert.Contains("GetSystemTimes", source, StringComparison.Ordinal);
        Assert.Contains("GetSystemPowerStatus", source, StringComparison.Ordinal);

        string settings = ReadRepositoryFile("app", "Settings.cs");
        Assert.Contains("System.Windows.Forms.Timer(components)", settings, StringComparison.Ordinal);
        Assert.Contains("hpLiveTelemetryTimer.Stop();", settings, StringComparison.Ordinal);
        Assert.Contains("hpLiveTelemetryProvider?.Reset();", settings, StringComparison.Ordinal);
        Assert.Contains("labelCPUFan.Click -= LabelCPUFan_Click;", settings, StringComparison.Ordinal);
        Assert.Contains("labelGPUFan.Click -= LabelCPUFan_Click;", settings, StringComparison.Ordinal);
        Assert.Contains("labelBattery.Click -= LabelBattery_Click;", settings, StringComparison.Ordinal);
        Assert.Contains("label.AccessibleRole = AccessibleRole.StaticText;", settings, StringComparison.Ordinal);
        Assert.Contains("if (AppConfig.IsHpVictusHardwareMode()) return;", settings, StringComparison.Ordinal);
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        string repositoryRoot = FindRepositoryRoot();
        return File.ReadAllText(Path.Combine([repositoryRoot, .. segments]));
    }

    private static string FindRepositoryRoot()
    {
        string? configuredRoot = Environment.GetEnvironmentVariable("VICTUSX_REPOSITORY_ROOT");
        if (!string.IsNullOrWhiteSpace(configuredRoot) && File.Exists(Path.Combine(configuredRoot, "VictusX.sln")))
        {
            return configuredRoot;
        }

        DirectoryInfo? directory = FindRepositoryRootFrom(AppContext.BaseDirectory);
        if (directory is not null)
        {
            return directory.FullName;
        }

        directory = FindRepositoryRootFrom(Directory.GetCurrentDirectory());
        if (directory is not null)
        {
            return directory.FullName;
        }

        throw new DirectoryNotFoundException("Could not locate the VictusX repository root.");
    }

    private static DirectoryInfo? FindRepositoryRootFrom(string startPath)
    {
        DirectoryInfo? directory = new(startPath);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "VictusX.sln")))
            {
                return directory;
            }

            directory = directory.Parent;
        }

        return null;
    }
}
