using System.Text;
using Application.Interfaces;
using Domain.Enums;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using PaymentGateway.Worker.Hubs;
using PaymentGateway.Worker.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;

namespace PaymentGateway.Worker.BackgroundTasks;

public class RabbitMqListener : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IHubContext<PaymentHub> _hubContext;
    private readonly ILogger<RabbitMqListener> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IChannel? _channel;
    private IConnection? _connection;

    public RabbitMqListener(IHubContext<PaymentHub> hubContext, IConfiguration configuration,
        ILogger<RabbitMqListener> logger, IConnectionFactory connectionFactory, IServiceProvider serviceProvider)
    {
        _hubContext = hubContext;
        _configuration = configuration;
        _logger = logger;
        _connectionFactory = connectionFactory;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var queueName = _configuration["RabbitMQ:Queue"] ?? "payment-gateway";
        _connection = await _connectionFactory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(queueName, ExchangeType.Fanout);

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += ReceiveRabbitMq;
        await _channel.QueueDeclareAsync(queueName, false, false, false);
        await _channel.BasicConsumeAsync(queueName, true, consumer);
        _logger.LogInformation("RabbitMqListener started");
    }

    private async Task ReceiveRabbitMq(object model, BasicDeliverEventArgs ea)
    {
        var body = ea.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var payload = JsonConvert.DeserializeObject<OrderMessage>(message);
        _logger.LogInformation("Notify for order: {orderId}", payload?.Id);
        await _hubContext.Clients.Group(payload?.UserId ?? "")
            .SendAsync("ReceiveNotifyMessage", message);
        if (payload != null && payload.Status == TransactionStatus.SUCCESS.GetValue())
        {
            // var notifyScope = _serviceProvider.CreateScope();
            // var notifyService = notifyScope.ServiceProvider.GetRequiredService<INotifyService>();
            // Send email notify
            // Get email by userId
            // await notifyService.SendEmail(email, subject, body);
            // =====================
            // Push to internal system
            // Get order by Id
            // await notifyService.PushInternalSystem(order);
        }
    }


    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }

        _logger.LogInformation("RabbitMqListener closed");
    }
}