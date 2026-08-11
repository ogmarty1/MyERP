using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<List<string>> GetUserRolesAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> CreateAsync(CreateUserRequest request);
        Task UpdateAsync(UpdateUserRequest request);
    }
}
