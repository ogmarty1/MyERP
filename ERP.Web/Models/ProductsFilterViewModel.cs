using System.ComponentModel.DataAnnotations;
using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ERP.Web.Models
{
    public class ProductsFilterViewModel
    {
        [Display(Name = "Keyword")]
        public string? Keyword { get; set; }

        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Supplier")]
        public int? SupplierId { get; set; }

        [Display(Name = "Min Price")]
        public decimal? MinPrice { get; set; }

        [Display(Name = "Max Price")]
        public decimal? MaxPrice { get; set; }

        [Display(Name = "Min Stock")]
        public int? MinStock { get; set; }

        [Display(Name = "Max Stock")]
        public int? MaxStock { get; set; }

        [Display(Name = "Status")]
        public ProductStatus? Status { get; set; }

        public bool LowStockOnly { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Suppliers { get; set; } = new List<SelectListItem>();
    }

    public class ProductsIndexViewModel
    {
        public ProductsFilterViewModel Filter { get; set; } = new();
        public List<Product> Products { get; set; } = new();
    }
}
