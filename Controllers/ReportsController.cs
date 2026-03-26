using DailyReports.Api.Data;
using DailyReports.Api.Dtos;
using DailyReports.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyReports.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<ReportDto>>> GetReports()
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    UserId = r.UserId,
                    WorkerName = r.User != null ? r.User.FullName : "",
                    Comment = r.Comment
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsByUser(int userId)
        {
            var reports = await _context.Reports
                .Where(r => r.UserId == userId)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    UserId = r.UserId,
                    WorkerName = r.User != null ? r.User.FullName : "",
                    Comment = r.Comment
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(CreateReportDto dto)
        {
            var user = await _context.Users.FindAsync(dto.UserId);

            if (user == null)
            {
                return BadRequest("Korisnik ne postoji.");
            }

            var report = new Report
            {
                ReportDate = dto.ReportDate,
                Location = dto.Location,
                Description = dto.Description,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                Comment = null
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return Ok(report);
        }

        [HttpPut("{id}/comment")]
        public async Task<ActionResult> UpdateComment(int id, UpdateCommentDto dto)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                return NotFound("Report not found.");
            }

            report.Comment = dto.Comment;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Comment saved successfully." });
        }
    }
}