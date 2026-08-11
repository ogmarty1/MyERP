using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class DashboardViewModel
    {
        public bool IsManagerOrAdmin { get; set; }

        // Employee stats
        public int MyOrdersCount { get; set; }

        // Manager / Admin KPIs
        public decimal MonthlyRevenue { get; set; }
        public int NewMonthlyOrdersCount { get; set; }
        public int TotalActiveCustomers { get; set; }
        public int CriticalStockCount { get; set; }
        public DateTime CurrentMonthStart { get; set; }
        public DateTime CurrentMonthEnd { get; set; }

        public List<Product> LowStockProducts { get; set; } = new();

        // Chart data (Manager / Admin only)
        public List<MonthlySalesPoint> MonthlySalesTrend { get; set; } = new();
        public List<TopSellingProductItem> TopSellingProducts { get; set; } = new();
    }
}
