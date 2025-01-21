using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RbacApi.Data;
using RbacApi.DTOs;
using RbacApi.Services;
using RbacApi.Models;

namespace RbacApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthService _authService;

        public AuthController(ApplicationDbContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDto>> Register(UserDto userDto)
        {
            if (_context.Users.Any(u => u.Username == userDto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                UserRoles = new List<UserRole>()
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = new List<string>()
            };

            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginDto.Username);
            if (user == null)
                return BadRequest("User not found");

            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return BadRequest("Wrong password");

            var token = _authService.GenerateJwtToken(user);
            return Ok(token);
        }
    }
}