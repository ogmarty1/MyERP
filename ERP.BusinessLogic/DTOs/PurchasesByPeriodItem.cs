namespace ERP.BusinessLogic.DTOs
{
    public class PurchasesByPeriodItem
    {
        public DateTime PeriodStart { get; set; }
        public string PeriodLabel { get; set; } = string.Empty;
        public int PurchaseOrderCount { get; set; }
        public decimal TotalSpend { get; set; }
    }
}
