using RabbitMQ.Client;

namespace Application.Interfaces;

public interface INotifyService
{
    Task<bool> RabbitmqPublish(IConnection conn, string queue, string routingKey, string message);
    Task<bool> SendEmail(string toEmail, string subject, string body);
    Task<bool> PushInternalSystem(Order order);
}