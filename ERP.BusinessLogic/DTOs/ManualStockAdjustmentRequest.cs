using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class ManualStockAdjustmentRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public MovementType MovementType { get; set; }
    }
}
