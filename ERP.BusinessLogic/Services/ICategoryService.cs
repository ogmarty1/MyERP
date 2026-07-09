using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface ICategoryService
    {
        Task<List<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(CreateCategoryRequest request);
        Task UpdateAsync(UpdateCategoryRequest request);
        Task DeleteAsync(int id);
    }
}
