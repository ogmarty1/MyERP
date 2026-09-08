using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ERP.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISupplierService _supplierService;
        private readonly IInventoryService _inventoryService;
        private readonly IPriceComparisonService _priceComparisonService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IInventoryService inventoryService,
            IPriceComparisonService priceComparisonService,
            IStringLocalizer<SharedResource> localizer)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _inventoryService = inventoryService;
            _priceComparisonService = priceComparisonService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(ProductsFilterViewModel filter)
        {
            var products = await _productService.GetFilteredAsync(new ProductFilterRequest
            {
                Keyword = filter.Keyword,
                CategoryId = filter.CategoryId,
                SupplierId = filter.SupplierId,
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                MinStock = filter.MinStock,
                MaxStock = filter.MaxStock,
                Status = filter.Status,
                LowStockOnly = filter.LowStockOnly
            });

            await PopulateFilterDropdownsAsync(filter);

            return View(new ProductsIndexViewModel { Filter = filter, Products = products });
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var viewModel = await BuildDetailsViewModelAsync(product);
            return View(viewModel);
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new ProductFormViewModel();
            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
            }

            await _productService.CreateAsync(new CreateProductRequest
            {
                Name = model.Name,
                Description = model.Description,
                SKU = model.SKU,
                PurchasePrice = model.PurchasePrice,
                SalePrice = model.SalePrice,
                QuantityInStock = model.QuantityInStock,
                MinimumQuantity = model.MinimumQuantity,
                IsActive = model.IsActive,
                Status = model.Status,
                Notes = model.Notes,
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId
            });

            TempData["SuccessMessage"] = _localizer["Product \"{0}\" was created successfully.", model.Name].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] ProductFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                    return NotFound();

                var viewModel = await BuildDetailsViewModelAsync(product, model);
                ViewData["ForceEditMode"] = true;
                return View("Details", viewModel);
            }

            await _productService.UpdateAsync(new UpdateProductRequest
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                SKU = model.SKU,
                PurchasePrice = model.PurchasePrice,
                SalePrice = model.SalePrice,
                QuantityInStock = model.QuantityInStock,
                MinimumQuantity = model.MinimumQuantity,
                IsActive = model.IsActive,
                Status = model.Status,
                Notes = model.Notes,
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId
            });

            TempData["SuccessMessage"] = _localizer["Product \"{0}\" was updated successfully.", model.Name].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _productService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = _localizer["This product cannot be deleted because it has existing order history."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Product was deleted successfully."].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdjustInventory(AdjustInventoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = _localizer["Please provide a valid quantity for the inventory adjustment."].Value;
                return RedirectToAction(nameof(Index));
            }

            try
            {
                await _inventoryService.AdjustStockAsync(new ManualStockAdjustmentRequest
                {
                    ProductId = model.ProductId,
                    Quantity = model.Quantity,
                    MovementType = model.MovementType,
                    Reason = model.Reason
                });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = _localizer["Inventory was adjusted successfully."].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckMarketPrice(int id)
        {
            try
            {
                var result = await _priceComparisonService.CheckPriceAsync(id);
                TempData["SuccessMessage"] = result.Offers.Count > 0
                    ? _localizer["Found {0} market price(s) for this product.", result.Offers.Count].Value
                    : _localizer["No market prices were found for this product."].Value;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = _localizer["Could not check market prices right now. Please try again later."].Value;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<ProductDetailsViewModel> BuildDetailsViewModelAsync(
            ERP.DataAccess.Models.Product product,
            ProductFormViewModel? formOverride = null)
        {
            var form = formOverride ?? new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                SKU = product.SKU,
                PurchasePrice = product.PurchasePrice,
                SalePrice = product.SalePrice,
                QuantityInStock = product.QuantityInStock,
                MinimumQuantity = product.MinimumQuantity,
                IsActive = product.IsActive,
                Status = product.Status,
                Notes = product.Notes,
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId
            };

            await PopulateDropdownsAsync(form);

            var orderHistory = await _productService.GetOrderHistoryAsync(product.Id);
            var priceHistory = await _priceComparisonService.GetHistoryAsync(product.Id);

            return new ProductDetailsViewModel
            {
                Form = form,
                OrderHistory = orderHistory,
                PriceHistory = priceHistory
            };
        }

        private async Task PopulateDropdownsAsync(ProductFormViewModel model)
        {
            var categories = await _categoryService.GetAllAsync();
            var suppliers = await _supplierService.GetAllAsync();

            model.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
            model.Suppliers = suppliers.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
        }

        private async Task PopulateFilterDropdownsAsync(ProductsFilterViewModel filter)
        {
            var categories = await _categoryService.GetAllAsync();
            var suppliers = await _supplierService.GetAllAsync();

            filter.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
            filter.Suppliers = suppliers.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
        }
    }
}
