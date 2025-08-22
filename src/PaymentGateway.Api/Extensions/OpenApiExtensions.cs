using Scalar.AspNetCore;

namespace PaymentGateway.Api.Extensions;

public static class OpenApiExtensions
{
    public static IServiceCollection AddConfigOpenApi(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            // options.CustomSchemaIds(type => type.FullName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Configuration", "App.xml");
            options.IncludeXmlComments(filePath, includeControllerXmlComments: true);
        });
        return services;
    }

    public static IApplicationBuilder UseOpenApiEndpoint(this WebApplication app)
    {
        app.MapControllers();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.MapScalarApiReference("docs", options =>
            {
                var descriptions = app.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    options.AddDocument(description.GroupName, title: $"API version {description.GroupName}", routePattern: $"/swagger/{description.GroupName}/swagger.json");
                }
                options.Title = "API Documents";
            });
        }
        return app;
    }
}