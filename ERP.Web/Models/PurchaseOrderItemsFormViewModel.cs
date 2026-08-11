namespace ERP.Web.Models
{
    public class PurchaseOrderItemsFormViewModel
    {
        public string? Notes { get; set; }
        public List<PurchaseOrderItemLineFormViewModel> Lines { get; set; } = new();
    }

    public class PurchaseOrderItemLineFormViewModel
    {
        public int? PurchaseOrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
