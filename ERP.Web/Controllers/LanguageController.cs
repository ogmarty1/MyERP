using ERP.Web.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Web.Controllers
{
    [AllowAnonymous]
    public class LanguageController : Controller
    {
        private static readonly string[] SupportedLanguages = { "en", "bg" };

        [HttpGet]
        public IActionResult Set(string culture, string? returnUrl = null)
        {
            if (Array.IndexOf(SupportedLanguages, culture) >= 0)
            {
                Response.Cookies.Append(UiCultureCookieProvider.CookieName, culture, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    IsEssential = true,
                    SameSite = SameSiteMode.Lax
                });
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
