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
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleResponseDto>>> GetRoles()
        {
            var roles = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Select(r => new RoleResponseDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Permissions = r.RolePermissions.Select(rp => rp.Permission.Name).ToList()
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleResponseDto>> GetRole(int id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            return new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
            };
        }

        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> CreateRole(RoleDto roleDto)
        {
            if (await _context.Roles.AnyAsync(r => r.Name == roleDto.Name))
                return BadRequest("Role name already exists");

            var role = new Role
            {
                Name = roleDto.Name,
                RolePermissions = new List<RolePermission>()
            };

            // Add permissions
            if (roleDto.PermissionIds.Any())
            {
                var permissions = await _context.Permissions
                    .Where(p => roleDto.PermissionIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var permission in permissions)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        Role = role,
                        Permission = permission,
                        RoleId = role.Id,
                        PermissionId = permission.Id
                    });
                }
            }

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, new RoleResponseDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, RoleDto roleDto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            if (await _context.Roles.AnyAsync(r => r.Name == roleDto.Name && r.Id != id))
                return BadRequest("Role name already exists");

            role.Name = roleDto.Name;

            // Update permissions
            _context.RolePermissions.RemoveRange(role.RolePermissions);
            
            if (roleDto.PermissionIds.Any())
            {
                var permissions = await _context.Permissions
                    .Where(p => roleDto.PermissionIds.Contains(p.Id))
                    .ToListAsync();

                foreach (var permission in permissions)
                {
                    role.RolePermissions.Add(new RolePermission
                    {
                        Role = role,
                        Permission = permission
                    });
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
                return NotFound();

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}