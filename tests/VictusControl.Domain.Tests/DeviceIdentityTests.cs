using VictusControl.Domain;

namespace VictusControl.Domain.Tests;

public sealed class DeviceIdentityTests
{
    [Fact]
    public void HpVictusIdentity_IsRecognizedAsKnownHpVictus()
    {
        var identity = new DeviceIdentity(
            DeviceManufacturer.Hp,
            DeviceFamily.Victus,
            "HP",
            "Victus Gaming Laptop 16-s0035nt",
            "7Z5Z2EA");

        Assert.True(identity.IsKnownHpVictus);
    }

    [Fact]
    public void UnknownIdentity_IsNotRecognizedAsKnownHpVictus()
    {
        Assert.False(DeviceIdentity.Unknown.IsKnownHpVictus);
        Assert.Equal(DeviceManufacturer.Unknown, DeviceIdentity.Unknown.Manufacturer);
        Assert.Equal(DeviceFamily.Unknown, DeviceIdentity.Unknown.Family);
    }
}
