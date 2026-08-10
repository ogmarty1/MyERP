using System.Globalization;

namespace ERP.Web.Helpers
{
    public static class CurrencyFormatter
    {
        private static readonly NumberFormatInfo EuroFormat = CreateEuroFormat();

        private static NumberFormatInfo CreateEuroFormat()
        {
            var format = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            format.CurrencySymbol = "€";
            format.CurrencyDecimalDigits = 2;
            format.CurrencyDecimalSeparator = ",";
            format.CurrencyGroupSeparator = " ";
            format.CurrencyPositivePattern = 3; // "n €"
            format.CurrencyNegativePattern = 8; // "-n €"
            return format;
        }

        // Display-only formatting using an explicit NumberFormatInfo, independent of the
        // ambient request culture, so it never affects decimal parsing on number inputs.
        public static string ToEuro(this decimal value) => value.ToString("C", EuroFormat);
    }
}
