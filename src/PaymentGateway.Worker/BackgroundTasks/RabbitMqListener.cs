using System.Text;
using Microsoft.AspNetCore.SignalR;
using PaymentGateway.Worker.Hubs;
using PaymentGateway.Worker.Models;
using RabbitMQ.Client;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace PaymentGateway.Worker.BackgroundTasks;

public class RabbitMqListener : BackgroundService
{
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqListener> _logger;
    private IConnectionFactory _connectionFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqListener(IHubContext<PaymentHub> hubContext, IConfiguration configuration, 
        ILogger<RabbitMqListener> logger, IConnectionFactory connectionFactory)
    {
        _hubContext = hubContext;
        _configuration = configuration;
        _logger = logger;
        _connectionFactory =connectionFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var queueName = _configuration["RabbitMQ:Queue"] ?? "payment-gateway";
        _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(exchange: queueName, type: ExchangeType.Fanout);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var payload = JsonConvert.DeserializeObject<OrderMessage>(message);
            _logger.LogInformation("Notify for order: {orderId}", payload?.Id);
            await _hubContext.Clients.Group(payload?.UserId ?? "")
                .SendAsync("ReceiveNotifyMessage", message, cancellationToken);
        };
        await _channel.QueueDeclareAsync( queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
        await _channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);
        _logger.LogInformation("RabbitMqListener started");
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken: cancellationToken);
            await _channel.DisposeAsync();
        }
        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken: cancellationToken);
            await _connection.DisposeAsync();
        }

        _logger.LogInformation("RabbitMqListener closed");
    }
}
