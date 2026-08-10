using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;

namespace ERP.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IDashboardService _dashboardService;

    public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
    {
        _logger = logger;
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        if (User.IsInRole("Manager") || User.IsInRole("Admin"))
        {
            var managerDashboard = await _dashboardService.GetManagerDashboardAsync();
            return View(managerDashboard);
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employeeDashboard = await _dashboardService.GetEmployeeDashboardAsync(userId);
        return View(employeeDashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("Home/Error/{statusCode:int?}")]
    public IActionResult Error(int? statusCode = null)
    {
        if (statusCode.HasValue)
        {
            Response.StatusCode = statusCode.Value;
            ViewData["StatusCode"] = statusCode.Value;
        }

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
