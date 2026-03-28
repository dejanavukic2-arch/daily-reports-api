namespace DailyReports.Api.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "";
        public string? Comment { get; set; }
        public string WorkerName { get; set; } = "";
        public string WorkerEmail { get; set; } = "";
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageBase64 { get; set; }
        public string? ImageMimeType { get; set; }
    }
}