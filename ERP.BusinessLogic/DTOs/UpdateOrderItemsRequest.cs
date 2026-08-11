namespace ERP.BusinessLogic.DTOs
{
    public class UpdateOrderItemsRequest
    {
        public int OrderId { get; set; }
        public string? Notes { get; set; }
        public List<UpdateOrderLineRequest> Lines { get; set; } = new();
    }

    public class UpdateOrderLineRequest
    {
        // Null for a line added during this edit; set for an existing OrderDetail being kept/changed.
        public int? OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
