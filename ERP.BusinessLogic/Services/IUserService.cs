using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<List<string>> GetUserRolesAsync(int userId);
    }
}
