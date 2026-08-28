namespace VictusControl.Domain;

public sealed class DeviceCapabilityProfile
{
    private readonly IReadOnlyDictionary<CapabilityKind, DeviceCapability> capabilitiesByKind;

    public DeviceCapabilityProfile(
        DeviceIdentity identity,
        IEnumerable<DeviceCapability>? capabilities = null,
        IEnumerable<string>? conflictSignals = null)
    {
        Identity = identity ?? throw new ArgumentNullException(nameof(identity));

        var capabilityList = (capabilities ?? Enumerable.Empty<DeviceCapability>())
            .GroupBy(capability => capability.Kind)
            .Select(group => group.Last())
            .ToArray();

        capabilitiesByKind = capabilityList.ToDictionary(capability => capability.Kind);
        Capabilities = Array.AsReadOnly(capabilityList);

        var conflicts = (conflictSignals ?? Enumerable.Empty<string>())
            .Where(signal => !string.IsNullOrWhiteSpace(signal))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        ConflictSignals = Array.AsReadOnly(conflicts);
    }

    public DeviceIdentity Identity { get; }

    public IReadOnlyList<DeviceCapability> Capabilities { get; }

    public IReadOnlyList<string> ConflictSignals { get; }

    public bool HasConflictRisk => ConflictSignals.Count > 0;

    public static DeviceCapabilityProfile Unknown(DeviceIdentity? identity = null) =>
        new(identity ?? DeviceIdentity.Unknown);

    public CapabilityStatus GetStatus(CapabilityKind kind) =>
        capabilitiesByKind.TryGetValue(kind, out var capability)
            ? capability.Status
            : CapabilityStatus.Unknown;

    public bool IsSupported(CapabilityKind kind) => GetStatus(kind) == CapabilityStatus.Supported;
}
