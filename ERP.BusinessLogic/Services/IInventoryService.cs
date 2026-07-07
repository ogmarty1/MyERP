using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IInventoryService
    {
        Task<InventoryMovement> AdjustStockAsync(ManualStockAdjustmentRequest request);
    }
}
