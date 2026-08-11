using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class ProductFilterRequest
    {
        public string? Keyword { get; set; }
        public int? CategoryId { get; set; }
        public int? SupplierId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinStock { get; set; }
        public int? MaxStock { get; set; }
        public ProductStatus? Status { get; set; }
        public bool LowStockOnly { get; set; }
    }
}
