using System.Reflection;
using System.Web;
using QRCoder;

namespace WebClient.Helpers;

public static class GlobalHelper
{
    public static string ToQRCodeImageSrc(this string text, int size)
    {
        using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
        using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q))
        using (PngByteQRCode qrCode = new PngByteQRCode(qrCodeData))
        {
            byte[] qrCodeImage = qrCode.GetGraphic(size);
            string base64String = Convert.ToBase64String(qrCodeImage);
            // Return data URI
            return $"data:image/png;base64,{base64String}";
        }
    }
    
    public static string ToQueryObject<T>(this T value)
    {
        var query = HttpUtility.ParseQueryString("");
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            //Setting column names as Property names
            query[prop.Name] = $"{prop.GetValue(value)}";
        }

        return query.ToString();
    }
    
    public static string FormatPriceVn(this decimal price)
    {
        return price.ToString("#,###").Replace(",", ".");
    }
}