using DailyReports.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DailyReports.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Report> Reports { get; set; }
    }
}