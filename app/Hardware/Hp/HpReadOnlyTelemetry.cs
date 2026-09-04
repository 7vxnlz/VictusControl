using System.ComponentModel;

namespace GHelper.Hardware.Hp;

internal readonly record struct HpCpuTimes(ulong Idle, ulong Kernel, ulong User);
internal readonly record struct HpPowerStatus(byte AcLineStatus, byte BatteryFlag, byte BatteryLifePercent);

internal interface IHpReadOnlyTelemetrySource
{
    HpCpuTimes? ReadCpuTimes();
    HpPowerStatus? ReadPowerStatus();
}

internal sealed record HpReadOnlyTelemetrySnapshot(
    DateTimeOffset? PolledAt,
    int? CpuLoadPercent,
    int? BatteryPercent,
    bool? BatteryPresent,
    bool? AcOnline,
    bool? Charging)
{
    // No verified temperature or tachometer source is available in this path.
    public double? CpuTemperatureCelsius => null;
    public double? GpuTemperatureCelsius => null;
    public int? FanRpm => null;

    public static HpReadOnlyTelemetrySnapshot Unavailable { get; } = new(null, null, null, null, null, null);
}

internal sealed class HpReadOnlyTelemetryProvider(IHpReadOnlyTelemetrySource source)
{
    private HpCpuTimes? previousCpu;
    private DateTimeOffset? previousCpuTime;
    internal static readonly TimeSpan MaximumSampleAge = TimeSpan.FromSeconds(5);

    public void Reset()
    {
        previousCpu = null;
        previousCpuTime = null;
    }

    public HpReadOnlyTelemetrySnapshot Capture(DateTimeOffset now)
    {
        HpCpuTimes? cpu = ReadSafely(source.ReadCpuTimes);
        int? load = CalculateLoad(cpu, now);
        HpPowerStatus? power = ReadSafely(source.ReadPowerStatus);
        bool? ac = power?.AcLineStatus switch { 0 => false, 1 => true, _ => null };
        bool? present = null;
        bool? charging = null;
        int? percent = null;
        if (power is { } value)
        {
            if (value.BatteryFlag == 128)
                present = false;
            else if (value.BatteryFlag <= 15)
            {
                present = true;
                percent = value.BatteryLifePercent <= 100 ? value.BatteryLifePercent : null;
                charging = (value.BatteryFlag & 8) != 0;
                if (ac == false && charging == true) charging = null;
            }
        }

        return new(now, load, percent, present, ac, charging);
    }

    private int? CalculateLoad(HpCpuTimes? current, DateTimeOffset now)
    {
        HpCpuTimes? previous = previousCpu;
        DateTimeOffset? previousTime = previousCpuTime;
        previousCpu = current;
        previousCpuTime = current.HasValue ? now : null;

        if (current is not { } next || previous is not { } before || previousTime is not { } sampledAt ||
            now <= sampledAt || now - sampledAt > MaximumSampleAge ||
            next.Idle < before.Idle || next.Kernel < before.Kernel || next.User < before.User)
            return null;

        // Kernel time includes idle time. Reject inconsistent counters instead of fabricating load.
        double total = (double)(next.Kernel - before.Kernel) + (next.User - before.User);
        double idle = next.Idle - before.Idle;
        if (total <= 0 || idle > total) return null;
        return (int)Math.Round(100 * (1 - idle / total));
    }

    private static T? ReadSafely<T>(Func<T?> read) where T : struct
    {
        try { return read(); }
        catch (Exception ex) when (ex is Win32Exception or InvalidOperationException or
            DllNotFoundException or EntryPointNotFoundException or BadImageFormatException or
            PlatformNotSupportedException or UnauthorizedAccessException)
        {
            return null;
        }
    }
}
