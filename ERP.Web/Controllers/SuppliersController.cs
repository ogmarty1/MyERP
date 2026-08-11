using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Models;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SuppliersController(ISupplierService supplierService, IStringLocalizer<SharedResource> localizer)
        {
            _supplierService = supplierService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(SuppliersFilterViewModel filter)
        {
            var suppliers = await _supplierService.GetFilteredAsync(new SupplierFilterRequest
            {
                Name = filter.Name,
                Company = filter.Company,
                ContactPerson = filter.ContactPerson,
                Phone = filter.Phone,
                Email = filter.Email
            });

            return View(new SuppliersIndexViewModel { Filter = filter, Suppliers = suppliers });
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();

            var purchaseOrders = await _supplierService.GetPurchaseOrderHistoryAsync(id);

            return View(BuildDetailsViewModel(supplier, purchaseOrders));
        }

        public IActionResult Create()
        {
            return View(new SupplierFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _supplierService.CreateAsync(new CreateSupplierRequest
            {
                Name = model.Name,
                CompanyName = model.CompanyName,
                ContactPerson = model.ContactPerson,
                TaxNumber = model.TaxNumber,
                Email = model.Email,
                Phone = model.Phone,
                IsActive = model.IsActive
            });

            TempData["SuccessMessage"] = _localizer["Supplier \"{0}\" was created successfully.", model.Name].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] SupplierFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var supplier = await _supplierService.GetByIdAsync(id);
                if (supplier == null)
                    return NotFound();

                var purchaseOrders = await _supplierService.GetPurchaseOrderHistoryAsync(id);
                var viewModel = BuildDetailsViewModel(supplier, purchaseOrders, model);
                ViewData["ForceEditMode"] = true;
                return View("Details", viewModel);
            }

            await _supplierService.UpdateAsync(new UpdateSupplierRequest
            {
                Id = model.Id,
                Name = model.Name,
                CompanyName = model.CompanyName,
                ContactPerson = model.ContactPerson,
                TaxNumber = model.TaxNumber,
                Email = model.Email,
                Phone = model.Phone,
                IsActive = model.IsActive
            });

            TempData["SuccessMessage"] = _localizer["Supplier \"{0}\" was updated successfully.", model.Name].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _supplierService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = _localizer["This supplier cannot be deleted because it is assigned to one or more products."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Supplier was deleted successfully."].Value;
            return RedirectToAction(nameof(Index));
        }

        private static SupplierDetailsViewModel BuildDetailsViewModel(
            Supplier supplier,
            List<PurchaseOrder> purchaseOrders,
            SupplierFormViewModel? formOverride = null)
        {
            var form = formOverride ?? new SupplierFormViewModel
            {
                Id = supplier.Id,
                Name = supplier.Name,
                CompanyName = supplier.CompanyName,
                ContactPerson = supplier.ContactPerson,
                TaxNumber = supplier.TaxNumber,
                Email = supplier.Email,
                Phone = supplier.Phone,
                IsActive = supplier.IsActive
            };

            return new SupplierDetailsViewModel
            {
                Form = form,
                PurchaseOrderHistory = purchaseOrders
            };
        }
    }
}
