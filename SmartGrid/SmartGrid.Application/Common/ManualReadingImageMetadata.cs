namespace SmartGrid.Application.Common
{
    public class ManualReadingImageMetadata
    {
        public Guid ReadingId { get; set; }
        public string Variant { get; set; } = string.Empty;
        public string FileExtension { get; set; } = "jpg";
    }
}
