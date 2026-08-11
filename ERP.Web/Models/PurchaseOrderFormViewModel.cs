using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP.Web.Models
{
    public class PurchaseOrderFormViewModel
    {
        [Required(ErrorMessage = "Please select a supplier.")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }

        public List<PurchaseOrderLineFormViewModel> Lines { get; set; } = new();

        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
        public List<Product> ProductCatalog { get; set; } = new();
    }

    public class PurchaseOrderLineFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive whole number.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be zero or greater.")]
        public decimal UnitPrice { get; set; }
    }
}
