using DailyReports.Api.Data;
using DailyReports.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DailyReports.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserListDto>>> GetUsers()
        {
            var users = await _context.Users
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.FullName)
                .Select(x => new UserListDto
                {
                    Id = x.Id,
                    FullName = x.FullName,
                    Email = x.Email,
                    Role = x.Role
                })
                .ToListAsync();

            return Ok(users);
        }
    }
}