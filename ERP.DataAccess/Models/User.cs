using System.ComponentModel.DataAnnotations;

namespace ERP.DataAccess.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        // Navigation property - потребителят може да има много роли
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        // Navigation property - потребителят може да е създал много поръчки
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
