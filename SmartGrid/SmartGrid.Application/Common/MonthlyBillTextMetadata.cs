namespace SmartGrid.Application.Common
{
    public class MonthlyBillTextMetadata
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string DeviceId { get; set; } = string.Empty;
    }
}
