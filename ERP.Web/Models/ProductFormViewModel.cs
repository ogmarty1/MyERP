using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP.Web.Models
{
    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(150, ErrorMessage = "Product name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "SKU is required.")]
        [StringLength(50, ErrorMessage = "SKU cannot exceed 50 characters.")]
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

        [Display(Name = "Status")]
        public ProductStatus Status { get; set; } = ProductStatus.Active;

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
    }
}
