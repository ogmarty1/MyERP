using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllAsync();
        Task<List<Supplier>> GetFilteredAsync(SupplierFilterRequest filter);
        Task<Supplier?> GetByIdAsync(int id);
        Task<List<PurchaseOrder>> GetPurchaseOrderHistoryAsync(int supplierId);
        Task<Supplier> CreateAsync(CreateSupplierRequest request);
        Task UpdateAsync(UpdateSupplierRequest request);
        Task DeleteAsync(int id);
    }
}
