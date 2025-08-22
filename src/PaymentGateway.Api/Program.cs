using Application;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Filters;
using PaymentGateway.Api.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddRabbitMQClient(connectionName: "rabbitmq");
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigApiVersion();
builder.Services.AddConfigOpenApi();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidateModelFilter>();
    options.Filters.Add<GlobalExceptionFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.ConfigureOptions<ConfigureSwaggerGenOptions>();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();
app.MapGet("/", () =>
{
    return "Service is running.";
}).WithMetadata(new ExcludeFromDescriptionAttribute());

app.UseOpenApiEndpoint();

// Init database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();