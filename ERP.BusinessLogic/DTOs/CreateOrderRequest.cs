namespace ERP.BusinessLogic.DTOs
{
    public class CreateOrderRequest
    {
        public string OrderNumber { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public int UserId { get; set; }
        public List<CreateOrderLineRequest> Lines { get; set; } = new();
    }

    public class CreateOrderLineRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Discount { get; set; }
    }
}
