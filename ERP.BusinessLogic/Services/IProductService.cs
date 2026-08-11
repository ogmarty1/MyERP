using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllAsync();
        Task<List<Product>> GetFilteredAsync(ProductFilterRequest filter);
        Task<Product?> GetByIdAsync(int id);
        Task<List<Order>> GetOrderHistoryAsync(int productId);
        Task<Product> CreateAsync(CreateProductRequest request);
        Task UpdateAsync(UpdateProductRequest request);
        Task DeleteAsync(int id);
    }
}
