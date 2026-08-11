using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP.Web.Models
{
    public class OrderFormViewModel
    {
        [Required(ErrorMessage = "Please select a customer.")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public List<OrderLineFormViewModel> Lines { get; set; } = new();

        public IEnumerable<SelectListItem> Customers { get; set; } = new List<SelectListItem>();
        public List<Product> ProductCatalog { get; set; } = new();
    }

    public class OrderLineFormViewModel
    {
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid product.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be a positive whole number.")]
        public int Quantity { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Unit price must be zero or greater.")]
        public decimal UnitPrice { get; set; }
    }
}
