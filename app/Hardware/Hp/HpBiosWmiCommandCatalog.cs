namespace GHelper.Hardware.Hp;

public static class HpBiosWmiCommandCatalog
{
    public static readonly IReadOnlyList<HpBiosWmiCommandDefinition> Definitions =
    [
        new(
            "SystemDesignData",
            HpBiosWmiCommandFamily.System,
            0x28,
            "hpqBIOSInt128",
            0,
            128,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent system design data command candidate. Do not invoke until a later explicit read-only command milestone."),

        new(
            "FanCount",
            HpBiosWmiCommandFamily.Fan,
            0x10,
            "hpqBIOSInt4",
            4,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent fan count command candidate. Does not imply fan control support."),

        new(
            "FanLevel",
            HpBiosWmiCommandFamily.Fan,
            0x2D,
            "hpqBIOSInt128",
            4,
            128,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent fan level command candidate. Manual fan writes remain forbidden."),

        new(
            "FanRpm",
            HpBiosWmiCommandFamily.Fan,
            0x38,
            "hpqBIOSInt128",
            4,
            128,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent direct fan RPM command candidate where firmware supports it."),

        new(
            "FanMaxState",
            HpBiosWmiCommandFamily.Fan,
            0x26,
            "hpqBIOSInt4",
            4,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent max fan state command candidate. Max fan writes remain forbidden."),

        new(
            "Temperature",
            HpBiosWmiCommandFamily.Thermal,
            0x23,
            "hpqBIOSInt4",
            4,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent temperature command candidate."),

        new(
            "GpuPowerState",
            HpBiosWmiCommandFamily.Gpu,
            0x21,
            "hpqBIOSInt4",
            4,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent GPU power state command candidate. GPU power writes remain forbidden."),

        new(
            "GpuMode",
            HpBiosWmiCommandFamily.Gpu,
            0x52,
            "hpqBIOSInt4",
            0,
            4,
            HpBiosWmiCommandAccess.Unknown,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent GPU mode command candidate. The same command ID is also used by write-capable flows in references, so it must stay gated."),

        new(
            "KeyboardType",
            HpBiosWmiCommandFamily.Keyboard,
            0x01,
            "hpqBIOSInt4",
            4,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent keyboard type command candidate."),

        new(
            "KeyboardBrightness",
            HpBiosWmiCommandFamily.Keyboard,
            0x04,
            "hpqBIOSInt4",
            4,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent keyboard brightness command candidate. Brightness writes remain forbidden."),

        new(
            "KeyboardColorTable",
            HpBiosWmiCommandFamily.Keyboard,
            0x02,
            "hpqBIOSInt128",
            1,
            128,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent keyboard color table command candidate. Color writes remain forbidden."),

        new(
            "LightBarSupport",
            HpBiosWmiCommandFamily.Lighting,
            0x01,
            "hpqBIOSInt4",
            0,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent light bar support command candidate. Lighting writes remain forbidden."),

        new(
            "LightBarRgb",
            HpBiosWmiCommandFamily.Lighting,
            0x04,
            "hpqBIOSInt128",
            0,
            128,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent light bar RGB state command candidate. RGB writes remain forbidden."),

        new(
            "DisplayOverdrive",
            HpBiosWmiCommandFamily.Display,
            0x35,
            "hpqBIOSInt4",
            0,
            4,
            HpBiosWmiCommandAccess.ReadOnly,
            HpBiosWmiCommandSafety.ReadIntent,
            "Read-intent display overdrive state command candidate. Display writes remain forbidden."),

        new(
            "FanLevelWrite",
            HpBiosWmiCommandFamily.Fan,
            0x2E,
            "hpqBIOSInt0",
            4,
            0,
            HpBiosWmiCommandAccess.WriteCapable,
            HpBiosWmiCommandSafety.Forbidden,
            "Forbidden fan level write command."),

        new(
            "FanModeWrite",
            HpBiosWmiCommandFamily.Fan,
            0x1A,
            "hpqBIOSInt0",
            2,
            0,
            HpBiosWmiCommandAccess.WriteCapable,
            HpBiosWmiCommandSafety.Forbidden,
            "Forbidden fan/performance mode write command."),

        new(
            "PowerLimitWrite",
            HpBiosWmiCommandFamily.Power,
            0x29,
            "hpqBIOSInt0",
            4,
            0,
            HpBiosWmiCommandAccess.WriteCapable,
            HpBiosWmiCommandSafety.Forbidden,
            "Forbidden power limit write command."),

        new(
            "GpuPowerWrite",
            HpBiosWmiCommandFamily.Gpu,
            0x22,
            "hpqBIOSInt0",
            4,
            0,
            HpBiosWmiCommandAccess.WriteCapable,
            HpBiosWmiCommandSafety.Forbidden,
            "Forbidden GPU power write command."),

        new(
            "KeyboardBacklightWrite",
            HpBiosWmiCommandFamily.Keyboard,
            0x05,
            "hpqBIOSInt0",
            1,
            0,
            HpBiosWmiCommandAccess.WriteCapable,
            HpBiosWmiCommandSafety.Forbidden,
            "Forbidden keyboard backlight or brightness write command."),

        new(
            "BatteryCareWrite",
            HpBiosWmiCommandFamily.Battery,
            0x24,
            "hpqBIOSInt0",
            4,
            0,
            HpBiosWmiCommandAccess.WriteCapable,
            HpBiosWmiCommandSafety.Forbidden,
            "Forbidden battery care or charge-limit write command.")
    ];
}
