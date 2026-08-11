using System.Security.Claims;
using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class PurchaseOrdersController : Controller
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        private readonly ISupplierService _supplierService;
        private readonly IProductService _productService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PurchaseOrdersController(
            IPurchaseOrderService purchaseOrderService,
            ISupplierService supplierService,
            IProductService productService,
            IStringLocalizer<SharedResource> localizer)
        {
            _purchaseOrderService = purchaseOrderService;
            _supplierService = supplierService;
            _productService = productService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var purchaseOrders = await _purchaseOrderService.GetAllAsync();
            return View(purchaseOrders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var purchaseOrder = await _purchaseOrderService.GetByIdAsync(id);
            if (purchaseOrder == null)
                return NotFound();

            var products = await _productService.GetAllAsync();

            return View(new PurchaseOrderDetailsViewModel { PurchaseOrder = purchaseOrder, ProductCatalog = products });
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new PurchaseOrderFormViewModel();
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrderFormViewModel model)
        {
            if (model.Lines == null || model.Lines.Count == 0)
            {
                ModelState.AddModelError(string.Empty, _localizer["A purchase order must contain at least one item."]);
            }
            else if (model.Lines.Any(l => l.ProductId <= 0 || l.Quantity <= 0))
            {
                ModelState.AddModelError(string.Empty, _localizer["All purchase order items must have a valid product and a positive quantity."]);
            }

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var purchaseOrder = await _purchaseOrderService.CreateAsync(new CreatePurchaseOrderRequest
            {
                SupplierId = model.SupplierId,
                CreatedByUserId = userId,
                Notes = model.Notes,
                Lines = model.Lines!.Select(l => new CreatePurchaseOrderLineRequest
                {
                    ProductId = l.ProductId,
                    Quantity = l.Quantity,
                    UnitPrice = l.UnitPrice
                }).ToList()
            });

            TempData["SuccessMessage"] = _localizer["Purchase order #{0} was created successfully.", purchaseOrder.Id].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditItems(int id, PurchaseOrderItemsFormViewModel model)
        {
            if (model.Lines.Count == 0)
            {
                TempData["ErrorMessage"] = _localizer["A purchase order must contain at least one item."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                await _purchaseOrderService.UpdateItemsAsync(new UpdatePurchaseOrderItemsRequest
                {
                    PurchaseOrderId = id,
                    Notes = model.Notes,
                    Lines = model.Lines.Select(l => new UpdatePurchaseOrderLineRequest
                    {
                        PurchaseOrderDetailId = l.PurchaseOrderDetailId,
                        ProductId = l.ProductId,
                        Quantity = l.Quantity,
                        UnitPrice = l.UnitPrice
                    }).ToList()
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Purchase order was updated successfully."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsReceived(int id)
        {
            try
            {
                await _purchaseOrderService.MarkAsReceivedAsync(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Purchase order was marked as received and stock was updated."].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task PopulateDropdownsAsync(PurchaseOrderFormViewModel model)
        {
            var suppliers = await _supplierService.GetAllAsync();
            var products = await _productService.GetAllAsync();

            model.Suppliers = suppliers.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
            model.ProductCatalog = products;
        }
    }
}
