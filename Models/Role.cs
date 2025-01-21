namespace RbacApi.Models
{
    public class Role
    {
        public Role()
        {
            UserRoles = new List<UserRole>();
            RolePermissions = new List<RolePermission>();
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
