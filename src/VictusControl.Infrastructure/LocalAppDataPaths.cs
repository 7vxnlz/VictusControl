namespace VictusControl.Infrastructure;

public static class LocalAppDataPaths
{
    public static string AppDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "VictusControl");

    public static string SettingsDirectory => AppDirectory;

    public static string LogDirectory => Path.Combine(AppDirectory, "Logs");
}
