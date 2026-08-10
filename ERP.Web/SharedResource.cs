namespace ERP.Web
{
    // Marker type used as the generic anchor for IStringLocalizer<SharedResource>.
    // Actual strings live in Resources/SharedResource.bg.resx (English text is
    // used directly as the resource key and is the fallback when no translation exists).
    // Must NOT live inside the "Resources" folder itself: with ResourcesPath="Resources"
    // configured, ASP.NET Core derives the resx base name from this type's namespace
    // relative to the project root and prepends the resources path, so a marker type
    // already namespaced under ERP.Web.Resources would double up to
    // "Resources.Resources.SharedResource" and silently fail to find any translation.
    public class SharedResource
    {
    }
}
