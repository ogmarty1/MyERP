using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IPurchaseOrderService
    {
        Task<List<PurchaseOrder>> GetAllAsync();
        Task<PurchaseOrder?> GetByIdAsync(int id);
        Task<PurchaseOrder> CreateAsync(CreatePurchaseOrderRequest request);
        Task UpdateItemsAsync(UpdatePurchaseOrderItemsRequest request);
        Task MarkAsReceivedAsync(int id);
    }
}
