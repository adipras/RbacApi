// Models/RolePermission.cs
namespace RbacApi.Models
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public required virtual Role Role { get; set; } = null!;
        public required virtual Permission Permission { get; set; } = null!;
    }
}
