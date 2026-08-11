namespace ERP.BusinessLogic.DTOs
{
    public class UpdatePurchaseOrderItemsRequest
    {
        public int PurchaseOrderId { get; set; }
        public string? Notes { get; set; }
        public List<UpdatePurchaseOrderLineRequest> Lines { get; set; } = new();
    }

    public class UpdatePurchaseOrderLineRequest
    {
        // Null for a line added during this edit; set for an existing PurchaseOrderDetail being kept/changed.
        public int? PurchaseOrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        // Only used when PurchaseOrderDetailId is null (new lines); existing lines keep their original price.
        public decimal UnitPrice { get; set; }
    }
}
