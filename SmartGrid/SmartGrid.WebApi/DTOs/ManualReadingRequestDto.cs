namespace SmartGrid.WebApi.DTOs
{
    public class ManualReadingRequestDto
    {
        public string DeviceId { get; set; } = string.Empty;
        public double ReadingKwh { get; set; }
        public DateTime ReadingAtUtc { get; set; }
        public string SubmitterEmail { get; set; } = string.Empty;
        public IFormFile? MeterImage { get; set; }
    }
}
