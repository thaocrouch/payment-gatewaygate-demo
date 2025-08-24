namespace Application.Interfaces;

public interface IOrderService
{
    Task<Order> GetOrderByIdAsync(string orderId);
    Task<OrderPaging> GetOrdersByUserIdAsync(string userId, int page = 1, int pageSize = 10);
    Task<OrderDTO> CreateOrderAsync(string userId, string name, decimal amount, int paymentMethod, int paymentSubType, DateTime expireDate);
    Task<bool> UpdateOrderAsync(string orderId, int status, int errorCode = 0, string? errorMessage = null);
    Task<bool> DeleteOrderAsync(string orderId);
    Task<OrderPaging> GetAllOrdersAsync(string name, int page = 1, int pageSize = 10);
}