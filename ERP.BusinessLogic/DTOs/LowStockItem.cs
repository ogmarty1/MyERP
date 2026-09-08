namespace ERP.BusinessLogic.DTOs
{
    public class LowStockItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int QuantityInStock { get; set; }
        public int MinimumQuantity { get; set; }
        public int Shortfall { get; set; }
    }
}
