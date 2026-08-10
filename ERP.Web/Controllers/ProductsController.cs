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
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            ISupplierService supplierService,
            IStringLocalizer<SharedResource> localizer)
        {
            _productService = productService;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
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
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId
            });

            TempData["SuccessMessage"] = _localizer["Product \"{0}\" was created successfully.", model.Name].Value;
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null)
                return NotFound();

            var viewModel = new ProductFormViewModel
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
                CategoryId = product.CategoryId,
                SupplierId = product.SupplierId
            };

            await PopulateDropdownsAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(model);
                return View(model);
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
                CategoryId = model.CategoryId,
                SupplierId = model.SupplierId
            });

            TempData["SuccessMessage"] = _localizer["Product \"{0}\" was updated successfully.", model.Name].Value;
            return RedirectToAction(nameof(Index));
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
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = _localizer["Product was deleted successfully."].Value;
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(ProductFormViewModel model)
        {
            var categories = await _categoryService.GetAllAsync();
            var suppliers = await _supplierService.GetAllAsync();

            model.Categories = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
            model.Suppliers = suppliers.Select(s => new SelectListItem(s.Name, s.Id.ToString()));
        }
    }
}
