using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class ManualStockAdjustmentRequest
    {
        public int ProductId { get; set; }

        // Inflow/Outflow: quantity to add/remove. Adjustment: the new absolute stock level.
        public int Quantity { get; set; }

        public MovementType MovementType { get; set; }
        public string? Reason { get; set; }
    }
}
