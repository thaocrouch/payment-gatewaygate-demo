using System.Globalization;
using System.Reflection;
using System.Web;
using Newtonsoft.Json;
using QRCoder;

namespace Shared;

public static class Helpers
{
    public static string ToQRCodeImageSrc(this string text, int size)
    {
        using (var qrGenerator = new QRCodeGenerator())
        using (var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
        using (var qrCode = new PngByteQRCode(qrCodeData))
        {
            var qrCodeImage = qrCode.GetGraphic(size);
            var base64String = Convert.ToBase64String(qrCodeImage);
            // Return data URI
            return $"data:image/png;base64,{base64String}";
        }
    }

    public static string ToQueryObject<T>(this T value)
    {
        var query = HttpUtility.ParseQueryString("");
        var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in Props)
            //Setting column names as Property names
            query[prop.Name] = $"{prop.GetValue(value)}";

        return query.ToString();
    }

    public static string FormatPriceVn(this decimal price)
    {
        return price.ToString("#,###").Replace(",", ".");
    }

    public static string ToJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public static T? ToObject<T>(this string input)
    {
        return JsonConvert.DeserializeObject<T>(input);
    }

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