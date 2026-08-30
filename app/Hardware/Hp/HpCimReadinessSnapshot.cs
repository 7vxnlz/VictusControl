namespace GHelper.Hardware.Hp;

public sealed record HpCimReadinessSnapshot(
    bool CimAvailable,
    bool CimRootWmiReachable,
    bool CimHpBIntMAvailable,
    bool CimHpBIntMMethodMetadataReadable,
    string[] CimErrors);
