using VictusControl.Domain;

namespace VictusControl.Domain.Tests;

public class DomainAssemblyMarkerTests
{
    [Fact]
    public void Domain_project_loads()
    {
        Assert.Equal("VictusControl.Domain", DomainAssemblyMarker.AssemblyName);
    }
}
