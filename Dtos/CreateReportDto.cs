namespace DailyReports.Api.Dtos
{
    public class CreateReportDto
    {
        public DateTime ReportDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int UserId { get; set; }
    }
}