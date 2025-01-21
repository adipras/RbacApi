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
    public class PermissionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PermissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PermissionResponseDto>>> GetPermissions()
        {
            var permissions = await _context.Permissions
                .Select(p => new PermissionResponseDto
                {
                    Id = p.Id,
                    Name = p.Name
                })
                .ToListAsync();

            return Ok(permissions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PermissionResponseDto>> GetPermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
                return NotFound();

            return new PermissionResponseDto
            {
                Id = permission.Id,
                Name = permission.Name
            };
        }

        [HttpPost]
        public async Task<ActionResult<PermissionResponseDto>> CreatePermission(PermissionDto permissionDto)
        {
            if (await _context.Permissions.AnyAsync(p => p.Name == permissionDto.Name))
                return BadRequest("Permission name already exists");

            var permission = new Permission
            {
                Name = permissionDto.Name,
                RolePermissions = new List<RolePermission>()
            };

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPermission), new { id = permission.Id }, new PermissionResponseDto
            {
                Id = permission.Id,
                Name = permission.Name
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePermission(int id, PermissionDto permissionDto)
        {
            var permission = await _context.Permissions.FindAsync(id);

            if (permission == null)
                return NotFound();

            if (await _context.Permissions.AnyAsync(p => p.Name == permissionDto.Name && p.Id != id))
                return BadRequest("Permission name already exists");

            permission.Name = permissionDto.Name;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePermission(int id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            _context.Permissions.Remove(permission);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
