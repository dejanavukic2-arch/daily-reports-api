namespace DailyReports.Api.Models
{
    public class Report
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string? Comment { get; set; }

        public User? User { get; set; }
    }
}