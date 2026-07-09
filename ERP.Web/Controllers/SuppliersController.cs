using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class SuppliersController : Controller
    {
        private readonly ISupplierService _supplierService;

        public SuppliersController(ISupplierService supplierService)
        {
            _supplierService = supplierService;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _supplierService.GetAllAsync();
            return View(suppliers);
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

            TempData["SuccessMessage"] = $"Supplier \"{model.Name}\" was created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();

            var viewModel = new SupplierFormViewModel
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

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

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

            TempData["SuccessMessage"] = $"Supplier \"{model.Name}\" was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _supplierService.GetByIdAsync(id);
            if (supplier == null)
                return NotFound();

            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _supplierService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "This supplier cannot be deleted because it is assigned to one or more products.");

                var supplier = await _supplierService.GetByIdAsync(id);
                return View(supplier);
            }

            TempData["SuccessMessage"] = "Supplier was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
