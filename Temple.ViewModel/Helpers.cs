using System.Globalization;

namespace Temple.ViewModel
{
    public static class Helpers
    {
        public static bool TryParse(
            this string text,
            out double? value,
            out string? error)
        {
            if (string.IsNullOrEmpty(text))
            {
                value = null;
                error = null;
                return true;
            }
            else if (double.TryParse(
                         text,
                         NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                         CultureInfo.InvariantCulture,
                         out var temp))
            {
                value = temp;
                error = null;
                return true;
            }
            else
            {
                value = double.NaN;
                error = "Invalid format";
                //_errors[propertyName] = "Invalid format";
                return false;
            }
        }
    }
}
