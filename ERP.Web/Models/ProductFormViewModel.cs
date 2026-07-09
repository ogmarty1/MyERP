using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP.Web.Models
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "SKU")]
        public string SKU { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be zero or greater.")]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Sale price must be zero or greater.")]
        [Display(Name = "Sale Price")]
        public decimal SalePrice { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity in stock must be zero or greater.")]
        [Display(Name = "Quantity In Stock")]
        public int QuantityInStock { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Minimum quantity must be zero or greater.")]
        [Display(Name = "Minimum Quantity")]
        public int MinimumQuantity { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
    }
}
