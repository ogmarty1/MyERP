namespace ERP.BusinessLogic.DTOs
{
    public class UpdateUserRequest
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        // Null/empty means "leave the current password unchanged".
        public string? Password { get; set; }

        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
