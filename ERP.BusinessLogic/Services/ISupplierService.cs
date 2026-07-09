using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface ISupplierService
    {
        Task<List<Supplier>> GetAllAsync();
    }
}
