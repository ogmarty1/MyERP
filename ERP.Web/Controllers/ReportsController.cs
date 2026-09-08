using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public async Task<IActionResult> Index(ReportsFilterViewModel filter, ReportType type = ReportType.SalesOverview)
        {
            var viewModel = new ReportsViewModel { Filter = filter, ActiveType = type };

            // Low Stock has no date range, so its filter validation/inputs don't apply.
            if (type != ReportType.LowStock && filter.StartDate.HasValue && filter.EndDate.HasValue && filter.StartDate > filter.EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start date must be on or before the end date.");
            }

            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            switch (type)
            {
                case ReportType.SalesOverview:
                    viewModel.SalesByPeriod = await _reportService.GetSalesByPeriodAsync(filter.StartDate, filter.EndDate, filter.Grouping);
                    viewModel.SalesByCategory = await _reportService.GetSalesByCategoryAsync(filter.StartDate, filter.EndDate);
                    break;
                case ReportType.PurchasesOverview:
                    viewModel.PurchasesByPeriod = await _reportService.GetPurchasesByPeriodAsync(filter.StartDate, filter.EndDate, filter.Grouping);
                    viewModel.PurchasesBySupplier = await _reportService.GetPurchasesBySupplierAsync(filter.StartDate, filter.EndDate);
                    break;
                case ReportType.LowStock:
                    viewModel.LowStock = await _reportService.GetLowStockAsync();
                    break;
                case ReportType.ProductProfitability:
                    viewModel.ProductProfitability = await _reportService.GetProductProfitabilityAsync(filter.StartDate, filter.EndDate);
                    break;
                case ReportType.TopCustomers:
                    viewModel.TopCustomers = await _reportService.GetTopCustomersAsync(filter.StartDate, filter.EndDate);
                    break;
                case ReportType.InventoryMovements:
                    viewModel.InventoryMovements = await _reportService.GetInventoryMovementsAsync(filter.StartDate, filter.EndDate);
                    break;
                case ReportType.SalesByEmployee:
                    viewModel.SalesByEmployee = await _reportService.GetSalesByEmployeeAsync(filter.StartDate, filter.EndDate);
                    break;
            }

            return View(viewModel);
        }
    }
}
