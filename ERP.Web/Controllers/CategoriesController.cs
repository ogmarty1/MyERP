using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Manager,Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
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

            TempData["SuccessMessage"] = $"Category \"{model.Name}\" was created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            var viewModel = new CategoryFormViewModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            await _categoryService.UpdateAsync(new UpdateCategoryRequest
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description
            });

            TempData["SuccessMessage"] = $"Category \"{model.Name}\" was updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(string.Empty,
                    "This category cannot be deleted because it is assigned to one or more products.");

                var category = await _categoryService.GetByIdAsync(id);
                return View(category);
            }

            TempData["SuccessMessage"] = "Category was deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
