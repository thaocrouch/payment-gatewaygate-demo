using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _dbContext;

    public OrderService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OrderDTO> CreateOrderAsync(string userId, string name, decimal amount, int paymentMethod, int paymentSubType, DateTime expireDate)
    {
        var result = new OrderDTO();
        var order = new Order(userId, name, amount, paymentMethod, paymentSubType, expireDate);
        _dbContext.Orders.Add(order);
        await _dbContext.SaveChangesAsync();
        // Fake response for banking, partner system.
        result.Id = order.Id;
        result.QrCode = $"users/pay/{order.Id}";
        result.PaymentUrl = $"users/pay/{order.Id}";
        return result;
    }

    public async Task<bool> DeleteOrderAsync(string orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order != null)
        {
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<OrderPaging> GetAllOrdersAsync(string name, int page = 1, int pageSize = 10)
    {
        var result = new OrderPaging();
        var offset = (page - 1) * pageSize;
        var orderQuery = _dbContext.Orders.AsNoTracking().AsQueryable()
            .Where(x => x.Name.Contains(name) | string.IsNullOrEmpty(name));
        var total = await orderQuery.CountAsync();
        if (total > 0)
        {
            result.Total = total;
            result.data = await orderQuery.OrderByDescending(x => x.CreatedDate).Skip(offset).Take(pageSize)
                .ToListAsync();
        }
        return result;
    }

    
    public async Task<Order> GetOrderByIdAsync(string orderId)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        return order;
    }

    public async Task<OrderPaging> GetOrdersByUserIdAsync(string userId, int page = 1, int pageSize = 10)
    {
        var result = new OrderPaging();
        var offset = (page - 1) * pageSize;
        var orderQuery = _dbContext.Orders.AsNoTracking().AsQueryable()
            .Where(x => x.UserId == userId);
        var total = await orderQuery.CountAsync();
        if (total > 0)
        {
            result.Total = total;
            result.data = await orderQuery.OrderByDescending(x => x.CreatedDate).Skip(offset).Take(pageSize)
                .ToListAsync();
        }
        return result;
    }

    public async Task<bool> UpdateOrderAsync(string orderId, int status, int errorCode = 0, string? errorMessage = null)
    {
        var order = await _dbContext.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.UpdateStatus(status, errorCode, errorMessage);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}
