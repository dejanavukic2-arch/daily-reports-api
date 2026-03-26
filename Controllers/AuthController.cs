using DailyReports.Api.Data;
using DailyReports.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyReports.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email && u.PasswordHash == dto.Password);

            if (user == null)
            {
                return Unauthorized("Pogrešan email ili lozinka.");
            }

            return Ok(new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                role = user.Role
            });
        }
    }
}