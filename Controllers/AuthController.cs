using DailyReports.Api.Data;
using DailyReports.Api.Dtos;
using DailyReports.Api.Models;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.FullName))
                    return BadRequest("Ime i prezime su obavezni.");

                if (string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest("Email je obavezan.");

                if (string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest("Lozinka je obavezna.");

                if (dto.Password.Length < 8)
                    return BadRequest("Lozinka mora imati najmanje 8 karaktera.");

                if (dto.Password != dto.ConfirmPassword)
                    return BadRequest("Lozinke se ne poklapaju.");

                var email = dto.Email.Trim().ToLower();
                var role = dto.Role?.Trim().ToLower() == "admin" ? "admin" : "worker";

                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (existingUser != null)
                    return BadRequest("Korisnik već postoji.");

                var user = new User
                {
                    FullName = dto.FullName.Trim(),
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    email = user.Email,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška na serveru: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                    return BadRequest("Email i lozinka su obavezni.");

                var email = dto.Email.Trim().ToLower();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                    return Unauthorized("Pogrešan email ili lozinka.");

                var isValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!isValidPassword)
                    return Unauthorized("Pogrešan email ili lozinka.");

                return Ok(new
                {
                    id = user.Id,
                    fullName = user.FullName,
                    email = user.Email,
                    role = user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška na serveru: {ex.Message}");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest("Email je obavezan.");

                if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                    return BadRequest("Trenutna lozinka je obavezna.");

                if (string.IsNullOrWhiteSpace(dto.NewPassword))
                    return BadRequest("Nova lozinka je obavezna.");

                if (dto.NewPassword.Length < 8)
                    return BadRequest("Nova lozinka mora imati najmanje 8 karaktera.");

                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return BadRequest("Nove lozinke se ne poklapaju.");

                var email = dto.Email.Trim().ToLower();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                    return NotFound("Korisnik ne postoji.");

                var isValidPassword = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
                if (!isValidPassword)
                    return BadRequest("Trenutna lozinka nije ispravna.");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                return Ok("Lozinka je uspešno promenjena.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Greška na serveru: {ex.Message}");
            }
        }
    }
}