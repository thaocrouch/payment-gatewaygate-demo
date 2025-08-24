using Microsoft.Extensions.Configuration;

namespace PaymentGateway.AppHost;

public static class AspireExtensions
{
    public static IResourceBuilder<T> WithConfigurationSection<T>(
        this IResourceBuilder<T> builder,
        IConfigurationSection section,
        string? prefix = null) where T : IResourceWithEnvironment
    {
        foreach (var child in section.GetChildren())
        {
            var key = string.IsNullOrEmpty(prefix) ? child.Key : $"{prefix}:{child.Key}";

            if (child.GetChildren().Any())
            {
                builder.WithConfigurationSection(child, key);
            }
            else
            {
                var envKey = key.Replace(":", "__");
                builder = builder.WithEnvironment(envKey, child.Value ?? "");
            }
        }

        return builder;
    }
}