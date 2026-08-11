using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllAsync();
        Task<List<Customer>> GetFilteredAsync(CustomerFilterRequest filter);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer> CreateAsync(CreateCustomerRequest request);
        Task UpdateAsync(UpdateCustomerRequest request);
        Task DeleteAsync(int id);
    }
}
