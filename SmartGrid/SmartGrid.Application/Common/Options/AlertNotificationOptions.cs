namespace SmartGrid.Application.Common.Options;

public sealed class AlertNotificationOptions
{
    public const string SectionName = "AlertNotifications";

    /// <summary>Comma-separated emails for critical grid alerts (voltage, offline, overload).</summary>
    public string NetworkAdminEmails { get; set; } = string.Empty;

    /// <summary>Critical low-voltage threshold (V) per specification example (e.g. 190V).</summary>
    public double CriticalVoltageThresholdVolts { get; set; } = 190;
}
