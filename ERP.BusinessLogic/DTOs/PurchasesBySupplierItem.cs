namespace ERP.BusinessLogic.DTOs
{
    public class PurchasesBySupplierItem
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = string.Empty;
        public int QuantityPurchased { get; set; }
        public decimal TotalSpend { get; set; }
    }
}
