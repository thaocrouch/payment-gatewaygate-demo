using Application.DTOs;
using Asp.Versioning;
using Domain.Enums;
using PaymentGateway.Api.Helpers;
using RabbitMQ.Client;

namespace PaymentGateway.Api.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class OrdersController(IOrderService orderService, INotifyService notifyService, IConfiguration configuration, IConnection conn) : ControllerBase
{
    [HttpPost(Name = "Create order")]
    public async Task<Result<OrderDTO>> Create([FromBody] CreateOrderRequest request)
    {
        var result = new Result<OrderDTO>();
        result.data = await orderService.CreateOrderAsync(
            request.UserId,
            request.Name,
            request.Amount,
            request.PaymentMethod,
            request.PaymentSubType,
            DateTime.UtcNow.AddMinutes(10) // Get value from config for production
        );

        return result;
    }
    
    [HttpPut("{id}", Name = "Update order")]
    public async Task<Result<bool>> Update(string id,  [FromBody] UpdateOrderRequest request)
    {
        var result = new Result<bool>();
        result.data = await orderService.UpdateOrderAsync(id, request.Status, request.ErrorCode, request.ErrorMessage);
        return result;
    }
    
    [HttpGet(Name = "Get orders")]
    public async Task<Result<OrderPaging>> GetOrders([FromQuery] FilterOrderRequest request)
    {
        var result = new Result<OrderPaging>();
        result.data = await orderService.GetAllOrdersAsync(request.Name, request.PageNumber, request.PageSize);
        return result;
    }

    [HttpGet("{id}", Name = "Get order by id")]
    [MapToApiVersion("1.0")]
    public async Task<Result<Order>> GetById(string id)
    {
        var result = new Result<Order>();
        result.data = await orderService.GetOrderByIdAsync(id);
        return result;
    }

    [HttpGet("{id}", Name = "Get order by id v2")]
    [MapToApiVersion("2.0")]
    public async Task<Result<Order>> GetByIdv2(string id)
    {
        var result = new Result<Order>();
        result.data = await orderService.GetOrderByIdAsync(id);
        return result;
    }
    
    [HttpDelete("{id}", Name = "Delete order")]
    public async Task<Result<bool>> Delete(string id)
    {
        var result = new Result<bool>();
        result.data = await orderService.DeleteOrderAsync(id);
        return result;
    }

    [HttpPost("ipn", Name = "IPN callback data")]
    public async Task<Result<bool>> Ipn(OrderIpnResponse response)
    {
        var result = new Result<bool>();
        var order = await orderService.GetOrderByIdAsync(response.RefId);
        var updateStatus = await orderService.UpdateOrderAsync(response.RefId, response.Status, response.ErrorCode, response.ErrorMessage);
        result.data = updateStatus;
        if (response.Status == TransactionStatus.SUCCESS.GetValue())
        {
            // Push to Queue
            // var host = $"{configuration["RabbitMq:Host"]}:{configuration["RabbitMq:Port"]}";
            var queueName = configuration["RabbitMq:Queue"];
            var message = new OrderMessageRequest
            {
                Id = response.RefId,
                Status = response.Status,
                UserId = order.UserId,
            };
            await notifyService.RabbitmqPublish(conn ,queueName, queueName, message.ToJson());
        }
        return result;
    }
}
