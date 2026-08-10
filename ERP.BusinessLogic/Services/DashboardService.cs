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
            var monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var monthEnd = monthStart.AddMonths(1);

            var monthlyRevenue = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled && o.OrderDate >= monthStart && o.OrderDate < monthEnd)
                .SumAsync(o => (decimal?)o.TotalAmount) ?? 0m;

            var newMonthlyOrdersCount = await _context.Orders
                .CountAsync(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd);

            var totalActiveCustomers = await _context.Customers
                .CountAsync(c => c.IsActive);

            var lowStockProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.QuantityInStock <= p.MinimumQuantity)
                .OrderBy(p => p.Name)
                .ToListAsync();

            var monthlySalesTrend = await GetMonthlySalesTrendAsync(monthStart);
            var topSellingProducts = await GetTopSellingProductsAsync();

            return new DashboardViewModel
            {
                IsManagerOrAdmin = true,
                MonthlyRevenue = monthlyRevenue,
                NewMonthlyOrdersCount = newMonthlyOrdersCount,
                TotalActiveCustomers = totalActiveCustomers,
                CriticalStockCount = lowStockProducts.Count,
                LowStockProducts = lowStockProducts,
                MonthlySalesTrend = monthlySalesTrend,
                TopSellingProducts = topSellingProducts
            };
        }

        private async Task<List<MonthlySalesPoint>> GetMonthlySalesTrendAsync(DateTime currentMonthStart)
        {
            var rangeStart = currentMonthStart.AddMonths(-11);

            var revenueByMonth = await _context.Orders
                .Where(o => o.Status != OrderStatus.Cancelled && o.OrderDate >= rangeStart)
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Revenue = g.Sum(o => o.TotalAmount) })
                .ToListAsync();

            var trend = new List<MonthlySalesPoint>();
            for (var month = rangeStart; month <= currentMonthStart; month = month.AddMonths(1))
            {
                var match = revenueByMonth.FirstOrDefault(m => m.Year == month.Year && m.Month == month.Month);
                trend.Add(new MonthlySalesPoint
                {
                    Label = month.ToString("MMM yyyy"),
                    Revenue = match?.Revenue ?? 0m
                });
            }

            return trend;
        }

        private async Task<List<TopSellingProductItem>> GetTopSellingProductsAsync()
        {
            return await _context.OrderDetails
                .Where(od => od.Order.Status != OrderStatus.Cancelled)
                .GroupBy(od => new { od.ProductId, od.Product.Name })
                .Select(g => new TopSellingProductItem
                {
                    ProductName = g.Key.Name,
                    Revenue = g.Sum(od => (od.UnitPrice * od.Quantity) - od.Discount)
                })
                .OrderByDescending(item => item.Revenue)
                .Take(5)
                .ToListAsync();
        }
    }
}
