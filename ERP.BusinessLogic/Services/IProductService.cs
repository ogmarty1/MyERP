using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(CreateProductRequest request);
        Task UpdateAsync(UpdateProductRequest request);
        Task DeleteAsync(int id);
    }
}
