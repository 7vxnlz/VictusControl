namespace GHelper.Hardware.Hp;

public enum HpVictusProbeAvailability
{
    Unknown,
    Available,
    Unavailable
}

public sealed record HpVictusCapabilitySnapshot(
    string Manufacturer,
    string Model,
    string SystemFamily,
    string SystemSku,
    string ProductVendor,
    string ProductName,
    string BiosVersion,
    bool IsHpManufacturer,
    bool IsVictusModel,
    HpVictusProbeAvailability RootWmiAvailability,
    HpVictusProbeAvailability HpqBIntMAvailability,
    HpVictusProbeAvailability HpqBDataInAvailability,
    string[] HpqBIntMMethodNames,
    string[] HpqBDataInMethodNames,
    string[] HpWmiReadOnlyClientErrors,
    bool InvocationSandboxAvailable,
    int SafeReadOnlyCommandCount,
    int RejectedCommandCount,
    string[] InvocationSandboxErrors,
    string[] Errors)
{
    public bool IsHpVictus => IsHpManufacturer && IsVictusModel;

    public string ToLogString()
    {
        string errors = Errors.Length == 0 ? "none" : string.Join(" | ", Errors);

        return $"Manufacturer='{Manufacturer}', Model='{Model}', Family='{SystemFamily}', SKU='{SystemSku}', ProductVendor='{ProductVendor}', ProductName='{ProductName}', BIOS='{BiosVersion}', IsHp={IsHpManufacturer}, IsVictus={IsVictusModel}, RootWmi={RootWmiAvailability}, hpqBIntM={HpqBIntMAvailability}, hpqBDataIn={HpqBDataInAvailability}, hpqBIntMMethods={HpqBIntMMethodNames.Length}, hpqBDataInMethods={HpqBDataInMethodNames.Length}, InvocationSandboxAvailable={InvocationSandboxAvailable}, SafeReadOnlyCommands={SafeReadOnlyCommandCount}, RejectedCommands={RejectedCommandCount}, Errors={errors}";
    }
}
