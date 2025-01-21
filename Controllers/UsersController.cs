using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RbacApi.Data;
using RbacApi.DTOs;
using RbacApi.Models;

namespace RbacApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsers()
        {
            var users = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    Roles = u.UserRoles.Select(ur => ur.Role.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost("{userId}/roles/{roleId}")]
        public async Task<IActionResult> AssignRoleToUser(int userId, int roleId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("User not found");
        
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
                return NotFound("Role not found");
        
            if (await _context.UserRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId))
                return BadRequest("User already has this role");
        
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId,
                User = user,
                Role = role
            };
        
            _context.UserRoles.Add(userRole);
            await _context.SaveChangesAsync();
        
            return NoContent();
        }
        
        [HttpDelete("{userId}/roles/{roleId}")]
        public async Task<IActionResult> RemoveRoleFromUser(int userId, int roleId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
        
            if (userRole == null)
                return NotFound();
        
            _context.UserRoles.Remove(userRole);
            await _context.SaveChangesAsync();
        
            return NoContent();
        }
        
        [HttpGet("{userId}/roles")]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetUserRoles(int userId)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);
        
            if (user == null)
                return NotFound();
        
            var roles = user.UserRoles.Select(ur => new RoleResponseDto
            {
                Id = ur.Role.Id,
                Name = ur.Role.Name,
                Permissions = ur.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
            });
        
            return Ok(roles);
        }
    }    
}
