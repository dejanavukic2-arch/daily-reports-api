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
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.Id)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    WorkerName = r.User != null ? r.User.FullName : "",
                    Comment = r.Comment,
                    UserId = r.UserId
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ReportDto>>> GetReportsByUser(int userId)
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.ReportDate)
                .ThenByDescending(r => r.Id)
                .Select(r => new ReportDto
                {
                    Id = r.Id,
                    ReportDate = r.ReportDate,
                    Location = r.Location,
                    Description = r.Description,
                    WorkerName = r.User != null ? r.User.FullName : "",
                    Comment = r.Comment,
                    UserId = r.UserId
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(CreateReportDto dto)
        {
            try
            {
                if (dto.UserId <= 0)
                    return BadRequest("UserId nije ispravan.");

                if (string.IsNullOrWhiteSpace(dto.Location))
                    return BadRequest("Lokacija je obavezna.");

                if (string.IsNullOrWhiteSpace(dto.Description))
                    return BadRequest("Opis je obavezan.");

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);

                if (user == null)
                    return BadRequest($"Korisnik sa ID {dto.UserId} ne postoji.");

                var report = new Report
                {
                    ReportDate = dto.ReportDate,
                    Location = dto.Location.Trim(),
                    Description = dto.Description.Trim(),
                    UserId = dto.UserId,
                    Comment = null
                };

                _context.Reports.Add(report);
                await _context.SaveChangesAsync();

                var createdReport = await _context.Reports
                    .Include(r => r.User)
                    .Where(r => r.Id == report.Id)
                    .Select(r => new ReportDto
                    {
                        Id = r.Id,
                        ReportDate = r.ReportDate,
                        Location = r.Location,
                        Description = r.Description,
                        WorkerName = r.User != null ? r.User.FullName : "",
                        Comment = r.Comment,
                        UserId = r.UserId
                    })
                    .FirstAsync();

                return Ok(createdReport);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška na serveru: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        [HttpPut("{id}/comment")]
        public async Task<IActionResult> UpdateComment(int id, UpdateCommentDto dto)
        {
            try
            {
                var report = await _context.Reports.FirstOrDefaultAsync(x => x.Id == id);

                if (report == null)
                    return NotFound("Izveštaj ne postoji.");

                report.Comment = dto.Comment?.Trim();
                await _context.SaveChangesAsync();

                return Ok("Komentar je uspešno sačuvan.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška na serveru: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
    }
}