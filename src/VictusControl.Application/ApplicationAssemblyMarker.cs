using VictusControl.Domain;
using VictusControl.Hardware.Abstractions;

namespace VictusControl.Application;

public static class ApplicationAssemblyMarker
{
    public static string DomainAssemblyName => DomainAssemblyMarker.AssemblyName;

    public static string HardwareAbstractionsAssemblyName => HardwareAbstractionsAssemblyMarker.AssemblyName;
}
