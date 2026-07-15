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

        public async Task<IActionResult> Index(ReportsFilterViewModel filter)
        {
            if (filter.StartDate.HasValue && filter.EndDate.HasValue && filter.StartDate > filter.EndDate)
            {
                ModelState.AddModelError(string.Empty, "Start date must be on or before the end date.");
            }

            var viewModel = new ReportsViewModel { Filter = filter };

            if (ModelState.IsValid)
            {
                viewModel.SalesByPeriod = await _reportService.GetSalesByPeriodAsync(filter.StartDate, filter.EndDate, filter.Grouping);
                viewModel.SalesByCategory = await _reportService.GetSalesByCategoryAsync(filter.StartDate, filter.EndDate);
            }

            return View(viewModel);
        }
    }
}
