using PaymentGateway.Worker.BackgroundTasks;
using PaymentGateway.Worker.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddRabbitMQClient(connectionName: "rabbitmq");
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapHub<PaymentHub>("/notify-payment-hub");
app.MapGet("/", () => "Worker node running");
app.Run();