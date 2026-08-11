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
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public CategoriesController(ICategoryService categoryService, IStringLocalizer<SharedResource> localizer)
        {
            _categoryService = categoryService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(BuildDetailsViewModel(category));
        }

        public IActionResult Create()
        {
            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.CreateAsync(new CreateCategoryRequest
            {
                Name = model.Name,
                Description = model.Description
            });

            TempData["SuccessMessage"] = _localizer["Category \"{0}\" was created successfully.", model.Name].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] CategoryFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                    return NotFound();

                var viewModel = BuildDetailsViewModel(category, model);
                ViewData["ForceEditMode"] = true;
                return View("Details", viewModel);
            }

            await _categoryService.UpdateAsync(new UpdateCategoryRequest
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            });

            TempData["SuccessMessage"] = _localizer["Category \"{0}\" was updated successfully.", model.Name].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = _localizer["This category cannot be deleted because it is assigned to one or more products."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["Category was deleted successfully."].Value;
            return RedirectToAction(nameof(Index));
        }

        private static CategoryDetailsViewModel BuildDetailsViewModel(Category category, CategoryFormViewModel? formOverride = null)
        {
            var form = formOverride ?? new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return new CategoryDetailsViewModel
            {
                Form = form,
                Products = category.Products.OrderBy(p => p.Name).ToList()
            };
        }
    }
}
