namespace ERP.BusinessLogic.DTOs
{
    public class TopCustomerItem
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }
}
