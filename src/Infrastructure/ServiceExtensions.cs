using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PaymentGateway");
        services.AddSingleton<DbConnected>(new DbConnected(connectionString ?? ""));
        services.AddDbContext<AppDbContext>(o => o.UseSqlServer(connectionString));
        return services;
    }
}