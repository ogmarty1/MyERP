using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Models
{
    public class CustomerFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(150)]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Tax Number")]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(150)]
        [EmailAddress]
        public string? Email { get; set; }

        [StringLength(30)]
        [Phone]
        public string? Phone { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
