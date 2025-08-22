using Domain.Enums;

namespace Domain.Entities;

public class Order
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string Note { get; set; }
    public int PaymentMethod { get; set; }
    public int PaymentSubType { get; set; }
    public int Status { get; set; }
    public string CallbackUrl { get; set; }
    public string IpnUrl { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime ExpireDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedDate { get; set; }

    public Order() { }

    public Order(string userId, string name, decimal amount, int paymentMethod, int paymentSubType, DateTime expireDate)
    {
        Id = Guid.NewGuid().ToString();
        UserId = userId;
        Name = name;
        Amount = amount;
        PaymentMethod = paymentMethod;
        PaymentSubType = paymentSubType;
        ExpireDate = expireDate;
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
        Status = (int) TransactionStatus.PENDING;
    }

    public void UpdateStatus(int status, int errorCode = 0, string? errorMessage = null)
    {
        Status = status;
        ErrorCode = errorCode;
        ErrorMessage = errorMessage ?? string.Empty;
        UpdatedDate = DateTime.UtcNow;
    }
}
