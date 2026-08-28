using VictusControl.Domain;

namespace VictusControl.Infrastructure;

public static class InfrastructureAssemblyMarker
{
    public static string DomainAssemblyName => DomainAssemblyMarker.AssemblyName;
}
