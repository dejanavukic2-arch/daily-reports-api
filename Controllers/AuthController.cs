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
        if (dto.Password != dto.ConfirmPassword)
            return BadRequest("Lozinke se ne poklapaju.");

        if (dto.Password.Length < 8)
            return BadRequest("Lozinka mora imati minimum 8 karaktera.");

        var existingUser = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == dto.Email);

        if (existingUser != null)
            return BadRequest("Korisnik već postoji.");

        var user = new User
        {
            FullName = dto.FirstName + " " + dto.LastName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Uspešna registracija.");
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
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
                fullName = $"{user.FirstName} {user.LastName}",
                user.Email,
                user.Role
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto dto)
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
    }
}