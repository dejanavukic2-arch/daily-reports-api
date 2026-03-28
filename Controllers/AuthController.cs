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
                if (string.IsNullOrWhiteSpace(dto.FirstName) || string.IsNullOrWhiteSpace(dto.LastName))
                    return BadRequest("Ime i prezime su obavezni.");

                if (string.IsNullOrWhiteSpace(dto.Email))
                    return BadRequest("Email je obavezan.");

                if (dto.Password != dto.ConfirmPassword)
                    return BadRequest("Lozinke se ne poklapaju.");

                if (dto.Password.Length < 8)
                    return BadRequest("Lozinka mora imati najmanje 8 karaktera.");

                var email = dto.Email.Trim().ToLower();

                var existingUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (existingUser != null)
                    return BadRequest("Korisnik već postoji.");

                var role = dto.Role?.Trim().ToLower() == "admin" ? "admin" : "worker";

                var user = new User
                {
                    FirstName = dto.FirstName.Trim(),
                    LastName = dto.LastName.Trim(),
                    Email = email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = role
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    user.Id,
                    fullName = user.FirstName + " " + user.LastName,
                    user.Email,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var email = dto.Email.Trim().ToLower();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                    return Unauthorized("Pogrešan email ili lozinka.");

                var validPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
                if (!validPassword)
                    return Unauthorized("Pogrešan email ili lozinka.");

                return Ok(new
                {
                    user.Id,
                    fullName = user.FirstName + " " + user.LastName,
                    user.Email,
                    user.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                var email = dto.Email.Trim().ToLower();

                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                    return NotFound("Korisnik ne postoji.");

                var validPassword = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
                if (!validPassword)
                    return BadRequest("Trenutna lozinka nije ispravna.");

                if (dto.NewPassword.Length < 8)
                    return BadRequest("Nova lozinka mora imati najmanje 8 karaktera.");

                if (dto.NewPassword != dto.ConfirmNewPassword)
                    return BadRequest("Nove lozinke se ne poklapaju.");

                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
                await _context.SaveChangesAsync();

                return Ok("Lozinka je uspešno promenjena.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}