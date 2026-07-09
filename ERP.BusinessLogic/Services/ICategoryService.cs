using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
    }
}
