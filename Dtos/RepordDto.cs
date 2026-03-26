namespace DailyReports.Api.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string WorkerName { get; set; } = string.Empty;
        public string? Comment { get; set; }
    }
}