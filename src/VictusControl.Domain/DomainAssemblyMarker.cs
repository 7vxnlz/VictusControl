namespace VictusControl.Domain;

public static class DomainAssemblyMarker
{
    public static string AssemblyName => typeof(DomainAssemblyMarker).Assembly.GetName().Name ?? string.Empty;
}
