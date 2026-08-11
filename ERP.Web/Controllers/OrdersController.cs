using System.Security.Claims;
using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Models;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace ERP.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public OrdersController(
            IOrderService orderService,
            IProductService productService,
            ICustomerService customerService,
            IStringLocalizer<SharedResource> localizer)
        {
            _orderService = orderService;
            _productService = productService;
            _customerService = customerService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(OrdersFilterViewModel filter)
        {
            var orders = await _orderService.GetFilteredAsync(new OrderFilterRequest
            {
                OrderId = filter.OrderId,
                CustomerName = filter.CustomerName,
                StartDate = filter.StartDate,
                EndDate = filter.EndDate,
                MinTotal = filter.MinTotal,
                MaxTotal = filter.MaxTotal,
                Status = filter.Status
            });

            return View(new OrdersIndexViewModel { Filter = filter, Orders = orders });
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound();

            var products = await _productService.GetAllAsync();

            return View(new OrderDetailsViewModel { Order = order, ProductCatalog = products });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, OrderStatus status)
        {
            try
            {
                await _orderService.ChangeStatusAsync(id, status);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Order status was updated successfully."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                await _orderService.CancelOrderAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Order was cancelled and its stock was restored."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItems(int id, OrderItemsFormViewModel model)
        {
            if (model.Lines.Count == 0)
            {
                TempData["ErrorMessage"] = _localizer["An order must contain at least one item."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _orderService.UpdateOrderItemsAsync(new UpdateOrderItemsRequest
                {
                    OrderId = id,
                    Notes = model.Notes,
                    Lines = model.Lines.Select(l => new UpdateOrderLineRequest
                    {
                        OrderDetailId = l.OrderDetailId,
                        ProductId = l.ProductId,
                        Quantity = l.Quantity
                    }).ToList()
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Order was updated successfully."].Value;
            return RedirectToAction(nameof(Details), new { id });
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
                ModelState.AddModelError(string.Empty, _localizer["An order must contain at least one item."]);
            }
            else if (model.Lines.Any(l => l.ProductId <= 0 || l.Quantity <= 0))
            {
                ModelState.AddModelError(string.Empty, _localizer["All order items must have a valid product and a positive quantity."]);
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await _orderService.CreateOrderAsync(new CreateOrderRequest
            {
                OrderNumber = orderNumber,
                CustomerId = model.CustomerId,
                UserId = userId,
                Lines = model.Lines!.Select(l => new CreateOrderLineRequest
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice,
                    Discount = 0m
                }).ToList()
            });

            TempData["SuccessMessage"] = _localizer["Order \"{0}\" was created successfully.", orderNumber].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInlineCustomer(CustomerFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(kvp => kvp.Value != null && kvp.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(new { errors });
            }

            var customer = await _customerService.CreateAsync(new CreateCustomerRequest
            {
                Name = model.Name,
                CompanyName = model.CompanyName,
                TaxNumber = model.TaxNumber,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                IsActive = true
            });

            return Json(new { id = customer.Id, name = customer.Name });
        }

        private async Task PopulateDropdownsAsync(OrderFormViewModel model)
        {
            var customers = await _customerService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            model.Customers = customers.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
            model.ProductCatalog = products;
        }
    }
}
