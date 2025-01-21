namespace RbacApi.Models
{
    public class Permission
    {
        public Permission()
        {
            RolePermissions = new List<RolePermission>();
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}
