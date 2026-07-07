using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class ManualStockAdjustmentRequest
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }
        public MovementType MovementType { get; set; }
    }
}
