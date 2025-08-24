namespace Application.DTOs;

public class OrderDTO
{
    /// <summary>
    ///     OrderId
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    ///     QRCode for payment
    /// </summary>
    public string QrCode { get; set; }

    /// <summary>
    ///     Payment on partner system
    /// </summary>
    public string PaymentUrl { get; set; }

    public DateTime ExpireDate { get; set; }
}