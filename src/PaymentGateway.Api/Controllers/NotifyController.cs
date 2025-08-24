// using PaymentGateway.Api.Helpers;
//
// namespace PaymentGateway.Api.Controllers;
//
// /// <summary>
// /// Just for test
// /// </summary>
// /// <param name="notifyService"></param>
// /// <param name="configuration"></param>
// [ApiVersion("1.0")]
// [ApiVersion("2.0")]
// [Route("api/v{version:apiVersion}/[controller]")]
// [ApiController]
// public class NotifyController(INotifyService notifyService, IConfiguration configuration) : ControllerBase
// {
//     [HttpPost("order")]
//     public async Task<string> Order(OrderMessageRequest order)
//     {
//         var host = $"{configuration["RabbitMq:Host"]}:{configuration["RabbitMq:Port"]}";
//         var queueName = configuration["RabbitMq:Queue"];
//         var result = await notifyService.RabbitmqPublish(queueName, queueName, order.ToJson());
//         return $"Order processed: {result}";
//     }
// }

