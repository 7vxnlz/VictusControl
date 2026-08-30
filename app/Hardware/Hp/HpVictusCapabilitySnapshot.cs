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
    string SystemDesignDataDryRunStatus,
    bool SystemDesignDataDryRunReady,
    string[] SystemDesignDataDryRunErrors,
    bool SystemDesignDataInvocationAllowed,
    bool SystemDesignDataInvocationAttempted,
    bool SystemDesignDataInvocationSucceeded,
    int SystemDesignDataReturnedByteCount,
    string SystemDesignDataInvocationError,
    bool SystemDesignDataDecodeSucceeded,
    string[] SystemDesignDataDecodeErrors,
    HpSystemDesignDataSnapshot? SystemDesignDataDecoded,
    bool FanGetCountInvocationAllowed,
    bool FanGetCountInvocationAttempted,
    bool FanGetCountInvocationSucceeded,
    int FanGetCountReturnedByteCount,
    string FanGetCountInvocationError,
    bool FanGetCountDecodeSucceeded,
    string[] FanGetCountDecodeErrors,
    HpFanGetCountSnapshot? FanGetCountDecoded,
    bool ProcessElevated,
    string WindowsIdentitySummary,
    bool WmiNamespaceReadable,
    bool HpBIntMClassReadable,
    bool HpBIntMMethodMetadataReadable,
    HpRelatedServiceSnapshot[] HpRelatedServices,
    string[] AccessDeniedInvestigationErrors,
    bool CimAvailable,
    bool CimRootWmiReachable,
    bool CimHpBIntMAvailable,
    bool CimHpBIntMMethodMetadataReadable,
    string[] CimErrors,
    bool HpWmiInvocationRequiresElevation,
    string HpWmiInvocationBlockedReason,
    string HpWmiRecommendedNextStep,
    string[] Errors)
{
    public bool IsHpVictus => IsHpManufacturer && IsVictusModel;

    public string ToLogString()
    {
        string errors = Errors.Length == 0 ? "none" : string.Join(" | ", Errors);

        return $"Manufacturer='{Manufacturer}', Model='{Model}', Family='{SystemFamily}', SKU='{SystemSku}', ProductVendor='{ProductVendor}', ProductName='{ProductName}', BIOS='{BiosVersion}', IsHp={IsHpManufacturer}, IsVictus={IsVictusModel}, RootWmi={RootWmiAvailability}, hpqBIntM={HpqBIntMAvailability}, hpqBDataIn={HpqBDataInAvailability}, hpqBIntMMethods={HpqBIntMMethodNames.Length}, hpqBDataInMethods={HpqBDataInMethodNames.Length}, InvocationSandboxAvailable={InvocationSandboxAvailable}, SafeReadOnlyCommands={SafeReadOnlyCommandCount}, RejectedCommands={RejectedCommandCount}, SystemDesignDataDryRunStatus={SystemDesignDataDryRunStatus}, SystemDesignDataDryRunReady={SystemDesignDataDryRunReady}, SystemDesignDataInvocationAllowed={SystemDesignDataInvocationAllowed}, SystemDesignDataInvocationAttempted={SystemDesignDataInvocationAttempted}, SystemDesignDataInvocationSucceeded={SystemDesignDataInvocationSucceeded}, SystemDesignDataReturnedByteCount={SystemDesignDataReturnedByteCount}, SystemDesignDataDecodeSucceeded={SystemDesignDataDecodeSucceeded}, FanGetCountInvocationAllowed={FanGetCountInvocationAllowed}, FanGetCountInvocationAttempted={FanGetCountInvocationAttempted}, FanGetCountInvocationSucceeded={FanGetCountInvocationSucceeded}, FanGetCountReturnedByteCount={FanGetCountReturnedByteCount}, FanGetCountDecodeSucceeded={FanGetCountDecodeSucceeded}, ProcessElevated={ProcessElevated}, WmiNamespaceReadable={WmiNamespaceReadable}, HpBIntMClassReadable={HpBIntMClassReadable}, HpBIntMMethodMetadataReadable={HpBIntMMethodMetadataReadable}, HpRelatedServices={HpRelatedServices.Length}, CimAvailable={CimAvailable}, CimRootWmiReachable={CimRootWmiReachable}, CimHpBIntMAvailable={CimHpBIntMAvailable}, CimHpBIntMMethodMetadataReadable={CimHpBIntMMethodMetadataReadable}, HpWmiInvocationRequiresElevation={HpWmiInvocationRequiresElevation}, HpWmiInvocationBlockedReason='{HpWmiInvocationBlockedReason}', Errors={errors}";
    }
}
