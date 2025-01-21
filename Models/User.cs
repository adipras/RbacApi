namespace RbacApi.Models
{
    public class User
    {
        public User()
        {
            UserRoles = new List<UserRole>();
        }

        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string PasswordHash { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
