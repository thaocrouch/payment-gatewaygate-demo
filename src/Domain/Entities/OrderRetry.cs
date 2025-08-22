namespace Domain.Entities;

public class OrderRetry
{
    public string OrderId { get; set; }
    public int Attempts { get; set; }
    public int ErrorCode { get; set; }
    public string ErrorMessage { get; set; }
}