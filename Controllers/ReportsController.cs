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
        public async Task<ActionResult<List<ReportDto>>> GetReports(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            [FromQuery] int? userId,
            [FromQuery] string? status)
        {
            var query = _context.Reports
                .Include(r => r.User)
                .AsQueryable();

            if (from.HasValue)
                query = query.Where(r => r.ReportDate.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(r => r.ReportDate.Date <= to.Value.Date);

            if (userId.HasValue)
                query = query.Where(r => r.UserId == userId.Value);

            if (!string.IsNullOrWhiteSpace(status))
            {
                var statusLower = status.Trim().ToLower();
                query = query.Where(r => r.Status.ToLower() == statusLower);
            }

            var reports = await query
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.CreatedAt)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    Status = r.Status,
                    Comment = r.Comment,
                    WorkerName = r.User != null ? r.User.FirstName + " " + r.User.LastName : "",
                    WorkerEmail = r.User != null ? r.User.Email : "",
                    UserId = r.UserId,
                    CreatedAt = r.CreatedAt,
                    ImageBase64 = r.ImageBase64,
                    ImageMimeType = r.ImageMimeType
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsForUser(int userId)
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.CreatedAt)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    Status = r.Status,
                    Comment = r.Comment,
                    WorkerName = r.User != null ? r.User.FirstName + " " + r.User.LastName : "",
                    WorkerEmail = r.User != null ? r.User.Email : "",
                    UserId = r.UserId,
                    CreatedAt = r.CreatedAt,
                    ImageBase64 = r.ImageBase64,
                    ImageMimeType = r.ImageMimeType
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(CreateReportDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
            if (user == null)
                return BadRequest("Korisnik ne postoji.");

            var report = new Report
            {
                ReportDate = dto.ReportDate,
                Location = dto.Location.Trim(),
                Description = dto.Description.Trim(),
                UserId = dto.UserId,
                Status = "na cekanju",
                ImageBase64 = dto.ImageBase64,
                ImageMimeType = dto.ImageMimeType
            };

            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            var result = await _context.Reports
                .Include(r => r.User)
                .Where(r => r.Id == report.Id)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    Status = r.Status,
                    Comment = r.Comment,
                    WorkerName = r.User != null ? r.User.FirstName + " " + r.User.LastName : "",
                    WorkerEmail = r.User != null ? r.User.Email : "",
                    UserId = r.UserId,
                    CreatedAt = r.CreatedAt,
                    ImageBase64 = r.ImageBase64,
                    ImageMimeType = r.ImageMimeType
                })
                .FirstAsync();

            return Ok(result);
        }

        [HttpPut("{id}/comment")]
        public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto dto)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == id);
            if (report == null)
                return NotFound("Izveštaj ne postoji.");

            report.Comment = dto.Comment?.Trim();
            await _context.SaveChangesAsync();

            return Ok("Komentar je sačuvan.");
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto dto)
        {
            var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == id);
            if (report == null)
                return NotFound("Izveštaj ne postoji.");

            var allowed = new[] { "na cekanju", "pregledano", "zavrseno" };
            var status = dto.Status.Trim().ToLower();

            if (!allowed.Contains(status))
                return BadRequest("Neispravan status.");

            report.Status = status;
            await _context.SaveChangesAsync();

            return Ok("Status je sačuvan.");
        }
    }
}