namespace ERP.BusinessLogic.DTOs
{
    public class SalesByPeriodItem
    {
        public DateTime PeriodStart { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
