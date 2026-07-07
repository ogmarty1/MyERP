using System.ComponentModel.DataAnnotations;

namespace ERP.DataAccess.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        // Navigation property - една роля може да е на много потребители
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
