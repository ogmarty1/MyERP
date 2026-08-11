using ERP.BusinessLogic.DTOs;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace ERP.BusinessLogic.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SalesByPeriodItem>> GetSalesByPeriodAsync(DateTime? startDate, DateTime? endDate, ReportPeriodGrouping grouping)
        {
            var query = _context.Orders.Where(o => o.Status != OrderStatus.Cancelled);

            if (startDate.HasValue)
                query = query.Where(o => o.OrderDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(o => o.OrderDate < endDate.Value.Date.AddDays(1));

            List<SalesByPeriodItem> results;

            if (grouping == ReportPeriodGrouping.Monthly)
            {
                var monthlyGroups = await query
                    .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalAmount)
                    })
                    .ToListAsync();

                results = monthlyGroups
                    .Select(g => new SalesByPeriodItem
                    {
                        PeriodStart = new DateTime(g.Year, g.Month, 1),
                        PeriodLabel = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                        OrderCount = g.OrderCount,
                        TotalRevenue = g.TotalRevenue
                    })
                    .OrderBy(item => item.PeriodStart)
                    .ToList();
            }
            else
            {
                results = await query
                    .GroupBy(o => o.OrderDate.Date)
                    .Select(g => new SalesByPeriodItem
                    {
                        PeriodStart = g.Key,
                        OrderCount = g.Count(),
                        TotalRevenue = g.Sum(o => o.TotalAmount)
                    })
                    .OrderBy(item => item.PeriodStart)
                    .ToListAsync();

                foreach (var item in results)
                    item.PeriodLabel = item.PeriodStart.ToString("yyyy-MM-dd");
            }

            return results;
        }

        public async Task<List<SalesByCategoryItem>> GetSalesByCategoryAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.OrderDetails.Where(od => od.Order.Status != OrderStatus.Cancelled);

            if (startDate.HasValue)
                query = query.Where(od => od.Order.OrderDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(od => od.Order.OrderDate < endDate.Value.Date.AddDays(1));

            return await query
                .GroupBy(od => new { od.Product.CategoryId, od.Product.Category.Name })
                .Select(g => new SalesByCategoryItem
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.Name,
                    QuantitySold = g.Sum(od => od.Quantity),
                    TotalRevenue = g.Sum(od => (od.UnitPrice * od.Quantity) - od.Discount)
                })
                .OrderByDescending(item => item.TotalRevenue)
                .ToListAsync();
        }

        public async Task<List<PurchasesByPeriodItem>> GetPurchasesByPeriodAsync(DateTime? startDate, DateTime? endDate, ReportPeriodGrouping grouping)
        {
            var query = _context.PurchaseOrders.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(po => po.OrderDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(po => po.OrderDate < endDate.Value.Date.AddDays(1));

            List<PurchasesByPeriodItem> results;

            if (grouping == ReportPeriodGrouping.Monthly)
            {
                var monthlyGroups = await query
                    .GroupBy(po => new { po.OrderDate.Year, po.OrderDate.Month })
                    .Select(g => new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        PurchaseOrderCount = g.Count(),
                        TotalSpend = g.Sum(po => po.TotalAmount)
                    })
                    .ToListAsync();

                results = monthlyGroups
                    .Select(g => new PurchasesByPeriodItem
                    {
                        PeriodStart = new DateTime(g.Year, g.Month, 1),
                        PeriodLabel = new DateTime(g.Year, g.Month, 1).ToString("MMMM yyyy"),
                        PurchaseOrderCount = g.PurchaseOrderCount,
                        TotalSpend = g.TotalSpend
                    })
                    .OrderBy(item => item.PeriodStart)
                    .ToList();
            }
            else
            {
                results = await query
                    .GroupBy(po => po.OrderDate.Date)
                    .Select(g => new PurchasesByPeriodItem
                    {
                        PeriodStart = g.Key,
                        PurchaseOrderCount = g.Count(),
                        TotalSpend = g.Sum(po => po.TotalAmount)
                    })
                    .OrderBy(item => item.PeriodStart)
                    .ToListAsync();

                foreach (var item in results)
                    item.PeriodLabel = item.PeriodStart.ToString("yyyy-MM-dd");
            }

            return results;
        }

        public async Task<List<PurchasesBySupplierItem>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.PurchaseOrderDetails.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(pod => pod.PurchaseOrder.OrderDate >= startDate.Value.Date);

            if (endDate.HasValue)
                query = query.Where(pod => pod.PurchaseOrder.OrderDate < endDate.Value.Date.AddDays(1));

            return await query
                .GroupBy(pod => new { pod.PurchaseOrder.SupplierId, pod.PurchaseOrder.Supplier.Name })
                .Select(g => new PurchasesBySupplierItem
                {
                    SupplierId = g.Key.SupplierId,
                    SupplierName = g.Key.Name,
                    QuantityPurchased = g.Sum(pod => pod.Quantity),
                    TotalSpend = g.Sum(pod => pod.UnitPrice * pod.Quantity)
                })
                .OrderByDescending(item => item.TotalSpend)
                .ToListAsync();
        }
    }
}
