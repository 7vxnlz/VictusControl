namespace VictusControl.Hardware.Abstractions;

public static class HardwareAbstractionsAssemblyMarker
{
    public static string AssemblyName => typeof(HardwareAbstractionsAssemblyMarker).Assembly.GetName().Name ?? string.Empty;
}
