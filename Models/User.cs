namespace DailyReports.Api.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "worker";

        public List<Report> Reports { get; set; } = new();
    }
}