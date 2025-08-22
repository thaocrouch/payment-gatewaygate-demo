using System.Globalization;

namespace PaymentGateway.Api.Extensions;

public static class Extensions
{
    public static int GetValue(this Enum e)
    {
        return Convert.ToInt32(e);
    }

    public static bool ValidDateTime(this string dateString)
    {
        return DateTime.TryParseExact(
            dateString,
            "yyyy-MM-dd HH:mm:ss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }
}