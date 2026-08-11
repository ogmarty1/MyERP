namespace ERP.BusinessLogic.DTOs
{
    public class CreatePurchaseOrderRequest
    {
        public int SupplierId { get; set; }
        public int CreatedByUserId { get; set; }
        public string? Notes { get; set; }
        public List<CreatePurchaseOrderLineRequest> Lines { get; set; } = new();
    }

    public class CreatePurchaseOrderLineRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
