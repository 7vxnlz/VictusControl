namespace GHelper.Hardware.Hp;

internal sealed record HpReadOnlyTelemetryDisplay(
    string Cpu, string Gpu, string FanAndDevice, string Battery, string Summary);

internal static class HpReadOnlyTelemetryFormatter
{
    public static HpReadOnlyTelemetryDisplay Format(
        HpReadOnlyTelemetrySnapshot snapshot, DateTimeOffset now, bool? hpVictusDetected, bool cachedIdentity)
    {
        bool fresh = snapshot.PolledAt is { } time && now >= time &&
            now - time <= HpReadOnlyTelemetryProvider.MaximumSampleAge;
        HpReadOnlyTelemetrySnapshot current = fresh ? snapshot : HpReadOnlyTelemetrySnapshot.Unavailable;
        string load = current.CpuLoadPercent is { } percent ? $"{percent}% load" : "Load: Unknown";
        string battery = current.BatteryPresent == false ? "No battery" :
            current.BatteryPercent is { } charge ? $"{charge}%" : "Unavailable";
        string ac = current.AcOnline switch { true => "AC", false => "On battery", _ => "AC unknown" };
        string charging = current.Charging switch { true => "Charging", false => "Not charging", _ => "Charging unknown" };
        string device = hpVictusDetected switch
        {
            true => "HP Victus detected",
            false => "HP Victus not detected",
            _ => "Device: Unknown"
        };
        string identitySource = cachedIdentity ? "cached report" : "startup snapshot";
        string state = snapshot.PolledAt is null ? "Not sampled" : fresh ? "Current" : "Stale";
        string poll = snapshot.PolledAt?.ToUniversalTime().ToString("u") ?? "Unavailable";
        string summary = $"Read-only OS telemetry: {state}; last poll: {poll}\n" +
            $"CPU load: {load} (GetSystemTimes); battery: {battery}, {ac}, {charging} (GetSystemPowerStatus).\n" +
            "CPU/GPU temperature and fan RPM: Unavailable; no verified sensor source.\n" +
            $"{device} ({identitySource}); cached fan levels remain raw-only. Normal fan control: NO-GO.";

        string batteryStatus = current.BatteryPresent == false ? $"No battery | {ac}" :
            current.AcOnline == true && current.Charging.HasValue ? $"{battery} | AC | {charging}" : $"{battery} | {ac}";
        return new(
            $"Temp: Unavailable | {load}", "Temp: Unavailable",
            $"Fan RPM: Unavailable | {device}" + (cachedIdentity ? " (cached)" : ""),
            batteryStatus, summary);
    }
}
