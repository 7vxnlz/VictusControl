namespace VictusControl.Domain;

public sealed record DeviceIdentity(
    DeviceManufacturer Manufacturer,
    DeviceFamily Family,
    string ManufacturerName,
    string Model,
    string? Sku = null,
    string? ProductName = null,
    string? BiosVersion = null)
{
    public static DeviceIdentity Unknown { get; } = new(
        DeviceManufacturer.Unknown,
        DeviceFamily.Unknown,
        string.Empty,
        string.Empty);

    public bool IsKnownHpVictus => Manufacturer == DeviceManufacturer.Hp && Family == DeviceFamily.Victus;
}
