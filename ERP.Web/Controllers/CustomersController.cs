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
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CustomersController(ICustomerService customerService, IStringLocalizer<SharedResource> localizer)
        {
            _customerService = customerService;
            _localizer = localizer;
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Index(CustomersFilterViewModel filter)
        {
            var customers = await _customerService.GetFilteredAsync(new CustomerFilterRequest
            {
                Name = filter.Name,
                Company = filter.Company,
                Phone = filter.Phone,
                Email = filter.Email,
                Address = filter.Address
            });

            return View(new CustomersIndexViewModel { Filter = filter, Customers = customers });
        }

        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();

            return View(BuildDetailsViewModel(customer));
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
                Notes = model.Notes,
                IsActive = model.IsActive
            });

            TempData["SuccessMessage"] = _localizer["Customer \"{0}\" was created successfully.", model.Name].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] CustomerFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                ViewData["ForceEditMode"] = true;
                return View("Details", new CustomerDetailsViewModel { Form = model });
            }

            await _customerService.UpdateAsync(new UpdateCustomerRequest
            {
                Id = model.Id,
                Name = model.Name,
                CompanyName = model.CompanyName,
                TaxNumber = model.TaxNumber,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                Notes = model.Notes,
                IsActive = model.IsActive
            });

            TempData["SuccessMessage"] = _localizer["Customer \"{0}\" was updated successfully.", model.Name].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = _localizer["This customer cannot be deleted because it has existing orders on record."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Customer was deleted successfully."].Value;
            return RedirectToAction(nameof(Index));
        }

        private static CustomerDetailsViewModel BuildDetailsViewModel(Customer customer)
        {
            return new CustomerDetailsViewModel
            {
                Form = new CustomerFormViewModel
                {
                    Id = customer.Id,
                    Name = customer.Name,
                    CompanyName = customer.CompanyName,
                    TaxNumber = customer.TaxNumber,
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Address = customer.Address,
                    Notes = customer.Notes,
                    IsActive = customer.IsActive
                }
            };
        }
    }
}
