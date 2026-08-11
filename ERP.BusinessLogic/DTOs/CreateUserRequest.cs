namespace ERP.BusinessLogic.DTOs
{
    public class CreateUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
