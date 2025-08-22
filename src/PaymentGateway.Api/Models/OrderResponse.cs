namespace PaymentGateway.Api.Models;

public class OrderIpnResponse
{
    public string TransactionId { get; set; }
    public string RefId { get; set; }
    public int Status { get; set; }
    public int ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
}