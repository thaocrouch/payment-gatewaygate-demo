using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqOption>(configuration.GetSection("RabbitMq"));
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<INotifyService, NotifyService>();
        return services;
    }
}