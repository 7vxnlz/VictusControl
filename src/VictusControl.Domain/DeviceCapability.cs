namespace VictusControl.Domain;

public sealed record DeviceCapability(
    CapabilityKind Kind,
    CapabilityStatus Status,
    string Reason = "")
{
    public bool IsSupported => Status == CapabilityStatus.Supported;

    public static DeviceCapability Unknown(CapabilityKind kind, string reason = "Capability has not been probed.") =>
        new(kind, CapabilityStatus.Unknown, reason);
}
