namespace DailyReports.Api.Dtos
{
    public class CreateReportDto
    {
        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public int UserId { get; set; }

        public string? ImageBase64 { get; set; }
        public string? ImageMimeType { get; set; }
    }
}