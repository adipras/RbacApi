namespace RbacApi.DTOs
{
    public class RoleDto
    {
        public required string Name { get; set; }
        public List<int> PermissionIds { get; set; } = new();
    }

    public class RoleResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<string> Permissions { get; set; } = new();
    }
}
