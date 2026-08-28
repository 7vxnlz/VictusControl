namespace VictusControl.Infrastructure.Windows;

public sealed record WindowsDeviceIdentitySnapshot(
    string? ManufacturerName = null,
    string? Model = null,
    string? SystemFamily = null,
    string? SystemSku = null,
    string? ProductName = null,
    string? ProductVendor = null,
    string? ProductVersion = null,
    string? ProductSku = null,
    string? BiosVersion = null);
