using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAllAsync();
            return View(customers);
        }

        [Authorize(Roles = "Manager,Admin")]
        public IActionResult Create()
        {
            return View(new CustomerFormViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _customerService.CreateAsync(new CreateCustomerRequest
            {
                Name = model.Name,
                CompanyName = model.CompanyName,
                TaxNumber = model.TaxNumber,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                IsActive = model.IsActive
            });

            TempData["SuccessMessage"] = $"Customer \"{model.Name}\" was created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            var viewModel = new CustomerFormViewModel
            {
                Id = customer.Id,
                Name = customer.Name,
                CompanyName = customer.CompanyName,
                TaxNumber = customer.TaxNumber,
                Email = customer.Email,
                Phone = customer.Phone,
                Address = customer.Address,
                IsActive = customer.IsActive
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CustomerFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _customerService.UpdateAsync(new UpdateCustomerRequest
            {
                Id = model.Id,
                Name = model.Name,
                CompanyName = model.CompanyName,
                TaxNumber = model.TaxNumber,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                IsActive = model.IsActive
            });

            TempData["SuccessMessage"] = $"Customer \"{model.Name}\" was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "This customer cannot be deleted because it has existing orders on record.");

                var customer = await _customerService.GetByIdAsync(id);
                return View(customer);
            }

            TempData["SuccessMessage"] = "Customer was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
