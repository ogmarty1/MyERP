using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetEmployeeDashboardAsync(int userId)
        {
            var myOrdersCount = await _context.Orders
                .CountAsync(o => o.UserId == userId);

            return new DashboardViewModel
            {
                IsManagerOrAdmin = false,
                MyOrdersCount = myOrdersCount
            };
        }

        public async Task<DashboardViewModel> GetManagerDashboardAsync()
        {
            var totalSalesRevenue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var pendingOrdersCount = await _context.Orders
                .CountAsync(o => o.Status == OrderStatus.Pending);

            var lowStockProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.QuantityInStock <= p.MinimumQuantity)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return new DashboardViewModel
            {
                IsManagerOrAdmin = true,
                TotalSalesRevenue = totalSalesRevenue,
                PendingOrdersCount = pendingOrdersCount,
                LowStockProducts = lowStockProducts
            };
        }
    }
}
