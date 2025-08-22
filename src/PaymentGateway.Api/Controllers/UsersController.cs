using Application.DTOs;
using Domain.Enums;

namespace PaymentGateway.Api.Controllers;

[ApiVersion("1.0")]
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class UsersController(IOrderService orderService) : ControllerBase
{
    // GET
    [HttpGet("orders", Name = "User Orders")]
    public async Task<Result<OrderPaging>> Get(UserFilterOrderRequest  request)
    {
        var result = new Result<OrderPaging>();
        result.data = await orderService.GetOrdersByUserIdAsync(request.UserId, request.PageNumber, request.PageSize);
        return result;
    }
}