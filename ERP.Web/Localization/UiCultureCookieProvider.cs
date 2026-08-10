using Microsoft.AspNetCore.Localization;

namespace ERP.Web.Localization
{
    // Reads the UI language from a dedicated cookie and applies it ONLY as the UI culture
    // (resource/string selection). The formatting/parsing culture is left untouched so
    // decimal number inputs on forms keep parsing with a period, regardless of language.
    public class UiCultureCookieProvider : RequestCultureProvider
    {
        public const string CookieName = "ERP_Lang";
        private static readonly string[] SupportedLanguages = { "en", "bg" };

        public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var lang = httpContext.Request.Cookies[CookieName];
            if (lang == null || Array.IndexOf(SupportedLanguages, lang) < 0)
                return Task.FromResult<ProviderCultureResult?>(null);

            // Culture (number/date formatting) always stays "en-US"; only UICulture (resource
            // selection) follows the cookie.
            return Task.FromResult<ProviderCultureResult?>(new ProviderCultureResult(culture: "en-US", uiCulture: lang));
        }
    }
}
