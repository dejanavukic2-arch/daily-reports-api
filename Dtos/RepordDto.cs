namespace DailyReports.Api.Dtos
{
    public class ReportDto
    {
        public int Id { get; set; }
        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = "";
        public string Description { get; set; } = "";
        public string WorkerName { get; set; } = "";
        public string? Comment { get; set; }
        public int UserId { get; set; }
    }
}