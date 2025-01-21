namespace RbacApi.DTOs
{
    public class PermissionDto
    {
        public required string Name { get; set; }
    }

    public class PermissionResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
}
