using System.ComponentModel.DataAnnotations;

namespace ERP.DataAccess.Models
{
    public class Supplier
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(50)]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
