namespace PaymentGateway.Api.Helpers;

public static class CommonHelper
{
    public static string ToJson(this object obj)
    {
        return JsonConvert.SerializeObject(obj);
    }
}