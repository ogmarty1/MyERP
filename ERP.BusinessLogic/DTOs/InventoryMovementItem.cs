using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class InventoryMovementItem
    {
        public DateTime Date { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public MovementType MovementType { get; set; }
        public int Quantity { get; set; }
        public string? Reason { get; set; }
        public string SourceReference { get; set; } = "-";
    }
}
