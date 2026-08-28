using VictusControl.Application;

namespace VictusControl.Application.Tests;

public class ApplicationAssemblyMarkerTests
{
    [Fact]
    public void Application_project_references_compile()
    {
        Assert.Equal("VictusControl.Domain", ApplicationAssemblyMarker.DomainAssemblyName);
        Assert.Equal("VictusControl.Hardware.Abstractions", ApplicationAssemblyMarker.HardwareAbstractionsAssemblyName);
    }
}
