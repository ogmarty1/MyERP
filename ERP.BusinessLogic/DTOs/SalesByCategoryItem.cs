namespace ERP.BusinessLogic.DTOs
{
    public class SalesByCategoryItem
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
