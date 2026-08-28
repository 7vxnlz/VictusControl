namespace VictusControl.Domain;

public enum CapabilityKind
{
    Unknown = 0,
    DeviceIdentity = 1,
    HpWmiNamespace = 2,
    FanControl = 3,
    ThermalMode = 4,
    KeyboardBacklight = 5,
    BatteryStatus = 6,
    PowerSource = 7,
    SensorTelemetry = 8,
    AcpiThermalZones = 9,
    HpServiceConflictDetection = 10
}
