using System.ComponentModel.DataAnnotations;

namespace ERP.Web.Models
{
    public class SupplierFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [StringLength(150, ErrorMessage = "Supplier name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Company name cannot exceed 150 characters.")]
        [Display(Name = "Company Name")]
        public string? CompanyName { get; set; }

        [StringLength(150, ErrorMessage = "Contact person cannot exceed 150 characters.")]
        [Display(Name = "Contact Person")]
        public string? ContactPerson { get; set; }

        [Required(ErrorMessage = "Tax number is required.")]
        [StringLength(50, ErrorMessage = "Tax number cannot exceed 50 characters.")]
        [Display(Name = "Tax Number")]
        public string TaxNumber { get; set; } = string.Empty;

        [StringLength(150, ErrorMessage = "Email cannot exceed 150 characters.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string? Email { get; set; }

        [StringLength(30, ErrorMessage = "Phone cannot exceed 30 characters.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string? Phone { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;
    }
}
