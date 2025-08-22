using RabbitMQ.Client;

namespace Application.Interfaces;

public interface INotifyService
{
    Task<bool> RabbitmqPublish(IConnection conn,string queue, string routingKey, string message);
    Task<bool> SendEmail(string email, string subject, string body);
    Task<bool> PushInternalSystem(Order order);
}