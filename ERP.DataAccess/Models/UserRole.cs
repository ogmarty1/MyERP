namespace ERP.DataAccess.Models
{
    // Join entity за Many-to-Many връзката между User и Role
    public class UserRole
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
