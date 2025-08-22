namespace PaymentGateway.Api.Common;

public class Result
{
    public int code { get; set; }
    public string message { get; set; }
    public object? data { get; set; }
}

public class Result<T>
{
    public int code { get; set; } = ApiCode.Success.GetValue();
    public string message { get; set; } = string.Empty;
    public T? data { get; set; }
}