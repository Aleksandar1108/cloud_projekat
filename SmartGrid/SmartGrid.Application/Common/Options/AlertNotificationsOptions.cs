namespace SmartGrid.Application.Common.Options
{
    public class AlertNotificationsOptions
    {
        public string NetworkAdminEmails { get; set; } = string.Empty;
        public double CriticalVoltageThresholdVolts { get; set; } = 190;
        public int OfflineThresholdMinutes { get; set; } = 35;
        public int VoltageAlertCooldownMinutes { get; set; } = 60;
    }
}
