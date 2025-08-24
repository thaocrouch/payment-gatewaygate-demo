using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptionDefinitions(configuration);
        services.AddHttpClient(HttpConnection.InternalSystem, (serviceProvider, option) =>
        {
            var internalSystemOption = serviceProvider.GetService<IOptions<InternalSystemOption>>();
            option.BaseAddress = new Uri(internalSystemOption?.Value.BaseUrl ?? "");
        });
        services.AddTransient<IOrderService, OrderService>();
        services.AddTransient<INotifyService, NotifyService>();
        return services;
    }

    public static void AddOptionDefinitions(this IServiceCollection services, IConfiguration configuration)
    {
        const string optionBaseSuffix = "Option";
        var optionBaseType = typeof(OptionBase);

        var optionBaseTypes = optionBaseType.Assembly.GetTypes()
            .Where(type => type != optionBaseType && optionBaseType.IsAssignableFrom(type)).ToList();

        optionBaseTypes.ForEach(settingType =>
        {
            var configureMethod = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethod(nameof(OptionsConfigurationServiceCollectionExtensions.Configure), new[] { typeof(IServiceCollection), typeof(IConfiguration) });
            var genericConfigureMethod = configureMethod.MakeGenericMethod(settingType);
            // Invoke Configure<T>(IConfiguration) for the setting type
            genericConfigureMethod.Invoke(null, new object[]
            {
                services, configuration.GetSection(settingType.Name.Replace(optionBaseSuffix, string.Empty))
            });
        });
    }
}