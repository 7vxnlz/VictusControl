namespace VictusControl.Application.Settings;

public readonly record struct SettingsVersion(int Major, int Minor, int Patch)
{
    public static SettingsVersion Current { get; } = new(1, 0, 0);

    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
