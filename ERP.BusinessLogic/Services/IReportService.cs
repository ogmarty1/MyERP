using ERP.BusinessLogic.DTOs;

namespace ERP.BusinessLogic.Services
{
    public interface IReportService
    {
        Task<List<SalesByPeriodItem>> GetSalesByPeriodAsync(DateTime? startDate, DateTime? endDate, ReportPeriodGrouping grouping);
        Task<List<SalesByCategoryItem>> GetSalesByCategoryAsync(DateTime? startDate, DateTime? endDate);
        Task<List<PurchasesByPeriodItem>> GetPurchasesByPeriodAsync(DateTime? startDate, DateTime? endDate, ReportPeriodGrouping grouping);
        Task<List<PurchasesBySupplierItem>> GetPurchasesBySupplierAsync(DateTime? startDate, DateTime? endDate);
    }
}
