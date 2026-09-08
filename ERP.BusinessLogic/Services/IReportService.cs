using ERP.BusinessLogic.DTOs;

namespace ERP.BusinessLogic.Services
{
    public interface IReportService
    {
        Task<List<SalesByPeriodItem>> GetSalesByPeriodAsync(DateTime? startDate, DateTime? endDate, ReportPeriodGrouping grouping);
        Task<List<SalesByCategoryItem>> GetSalesByCategoryAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PurchasesByPeriodItem>> GetPurchasesByPeriodAsync(DateTime? startDate, DateTime? endDate, ReportPeriodGrouping grouping);
        Task<List<PurchasesBySupplierItem>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate);
        Task<List<LowStockItem>> GetLowStockAsync();
        Task<List<ProductProfitabilityItem>> GetProductProfitabilityAsync(DateTime? startDate, DateTime? endDate);
        Task<List<TopCustomerItem>> GetTopCustomersAsync(DateTime? startDate, DateTime? endDate);
        Task<List<InventoryMovementItem>> GetInventoryMovementsAsync(DateTime? startDate, DateTime? endDate);
        Task<List<SalesByEmployeeItem>> GetSalesByEmployeeAsync(DateTime? startDate, DateTime? endDate);
    }
}
