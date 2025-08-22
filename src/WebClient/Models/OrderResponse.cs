namespace WebClient.Models;

public class CreateOrderResponse
{
    public string Id { get; set; }
    public string QrCode { get; set; }
    public string PaymentUrl { get; set; }
    public DateTime ExpireDate { get; set; }
}