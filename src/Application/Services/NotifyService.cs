using System.Text;
using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Application.Services;

public class NotifyService : INotifyService
{
    private readonly ILogger<NotifyService> _logger;
    private readonly RabbitMqOption _rabbitMqOption;

    public NotifyService(ILogger<NotifyService> logger, IOptionsSnapshot<RabbitMqOption> rabbitMqOption)
    {
        _logger = logger;
        _rabbitMqOption = rabbitMqOption.Value;
    }

    public async Task<bool> RabbitmqPublish(IConnection conn, string queue, string routingKey, string message)
    {
        try
        {
            using var channel = await conn.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue: queue, durable: false, exclusive: false, autoDelete: false);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync(exchange: "", routingKey: routingKey, body: body);
        }
        catch (Exception e)
        {
            _logger.LogError("Can't publish message to Rabbitmq: url : {url} - message: {message} - error: {ex}", _rabbitMqOption.Host, message, e);
            return false;
        }
        return true;
    }

    public Task<bool> SendEmail(string email, string subject, string body)
    {
        // Send email to users
        return Task.FromResult(true);
    }

    public Task<bool> PushInternalSystem(Order order)
    {
        // Push order to internal system.
        return Task.FromResult(true);
    }
}