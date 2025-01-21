// Models/UserRole.cs
namespace RbacApi.Models
{
    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public required virtual User User { get; set; } = null!;
        public required virtual Role Role { get; set; } = null!;
    }
}
