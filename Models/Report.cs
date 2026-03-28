namespace DailyReports.Api.Models
{
    public class Report
    {
        public int Id { get; set; }

        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public string Status { get; set; } = "na cekanju";
        public string? Comment { get; set; }

        public string? ImageBase64 { get; set; }
        public string? ImageMimeType { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int UserId { get; set; }
        public User? User { get; set; }
    }
}