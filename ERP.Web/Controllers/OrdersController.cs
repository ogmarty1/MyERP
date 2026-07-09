using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Data;
using ERP.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly ApplicationDbContext _context;

        public OrdersController(
            IOrderService orderService,
            IProductService productService,
            ICustomerService customerService,
            ApplicationDbContext context)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetAllAsync();
            return View(orders);
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new OrderFormViewModel();
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderFormViewModel model)
        {
            if (model.Lines == null || model.Lines.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "An order must contain at least one item.");
            }
            else if (model.Lines.Any(l => l.ProductId <= 0 || l.Quantity <= 0))
            {
                ModelState.AddModelError(string.Empty, "All order items must have a valid product and a positive quantity.");
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";

            await _orderService.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = orderNumber,
                CustomerId = model.CustomerId,
                UserId = model.UserId,
                Lines = model.Lines!.Select(l => new CreateOrderLineRequest
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    Discount = 0m
                }).ToList()
            });

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(OrderFormViewModel model)
        {
            var customers = await _customerService.GetAllAsync();
            var users = await _context.Users.OrderBy(u => u.Username).ToListAsync();
            var products = await _productService.GetAllAsync();

            model.Customers = customers.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
            model.Users = users.Select(u => new SelectListItem(u.Username, u.Id.ToString()));
            model.ProductCatalog = products;
        }
    }
}
