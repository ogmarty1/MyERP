namespace ERP.BusinessLogic.DTOs
{
    public class CreateProductRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SKU { get; set; } = string.Empty;
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public int QuantityInStock { get; set; }
        public int MinimumQuantity { get; set; }
        public bool IsActive { get; set; } = true;
        public int CategoryId { get; set; }
        public int? SupplierId { get; set; }
    }
}
