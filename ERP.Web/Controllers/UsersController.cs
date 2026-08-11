using ERP.BusinessLogic.DTOs;
using ERP.BusinessLogic.Services;
using ERP.DataAccess.Data;
using ERP.DataAccess.Models;
using ERP.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace ERP.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UsersController(IUserService userService, ApplicationDbContext context, IStringLocalizer<SharedResource> localizer)
        {
            _userService = userService;
            _context = context;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllAsync();
            return View(users);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(await BuildDetailsViewModelAsync(user));
        }

        public async Task<IActionResult> Create()
        {
            var viewModel = new UserFormViewModel();
            await PopulateRolesAsync(viewModel);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(nameof(model.Password), _localizer["Password is required."]);
            }

            if (!ModelState.IsValid)
            {
                await PopulateRolesAsync(model);
                return View(model);
            }

            try
            {
                await _userService.CreateAsync(new CreateUserRequest
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password!,
                    Phone = model.Phone,
                    RoleId = model.RoleId,
                    IsActive = model.IsActive
                });
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError(nameof(model.Email), _localizer["A user with this email already exists."]);
                await PopulateRolesAsync(model);
                return View(model);
            }

            TempData["SuccessMessage"] = _localizer["User \"{0}\" was created successfully.", $"{model.FirstName} {model.LastName}"].Value;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] UserFormViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                    return NotFound();

                var viewModel = await BuildDetailsViewModelAsync(user, model);
                ViewData["ForceEditMode"] = true;
                return View("Details", viewModel);
            }

            try
            {
                await _userService.UpdateAsync(new UpdateUserRequest
                {
                    Id = model.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Password = model.Password,
                    Phone = model.Phone,
                    RoleId = model.RoleId,
                    IsActive = model.IsActive
                });
            }
            catch (DbUpdateException)
            {
                TempData["ErrorMessage"] = _localizer["A user with this email already exists."].Value;
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["SuccessMessage"] = _localizer["User \"{0}\" was updated successfully.", $"{model.FirstName} {model.LastName}"].Value;
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<UserDetailsViewModel> BuildDetailsViewModelAsync(User user, UserFormViewModel? formOverride = null)
        {
            var form = formOverride ?? new UserFormViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Phone = user.Phone,
                RoleId = user.UserRoles.Select(ur => ur.RoleId).FirstOrDefault(),
                IsActive = user.IsActive
            };

            await PopulateRolesAsync(form);

            return new UserDetailsViewModel { Form = form };
        }

        private async Task PopulateRolesAsync(UserFormViewModel model)
        {
            var roles = await _context.Roles.OrderBy(r => r.Name).ToListAsync();
            model.Roles = roles.Select(r => new SelectListItem(r.Name, r.Id.ToString()));
        }
    }
}
