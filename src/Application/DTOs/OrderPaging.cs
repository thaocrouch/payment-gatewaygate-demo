namespace Application.DTOs;

public class OrderPaging
{
    public int Total { get; set; }
    public IEnumerable<Order> data { get; set; }
}