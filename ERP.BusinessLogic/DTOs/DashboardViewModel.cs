using ERP.DataAccess.Models;

namespace ERP.BusinessLogic.DTOs
{
    public class DashboardViewModel
    {
        public bool IsManagerOrAdmin { get; set; }

        // Employee stats
        public int MyOrdersCount { get; set; }

        // Manager / Admin stats
        public decimal TotalSalesRevenue { get; set; }
        public int PendingOrdersCount { get; set; }
        public List<Product> LowStockProducts { get; set; } = new();
    }
}
