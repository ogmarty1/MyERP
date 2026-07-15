using ERP.BusinessLogic.DTOs;

namespace ERP.BusinessLogic.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetEmployeeDashboardAsync(int userId);
        Task<DashboardViewModel> GetManagerDashboardAsync();
    }
}
