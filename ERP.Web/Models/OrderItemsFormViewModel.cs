namespace ERP.Web.Models
{
    public class OrderItemsFormViewModel
    {
        public string? Notes { get; set; }
        public List<OrderItemLineFormViewModel> Lines { get; set; } = new();
    }

    public class OrderItemLineFormViewModel
    {
        public int? OrderDetailId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
