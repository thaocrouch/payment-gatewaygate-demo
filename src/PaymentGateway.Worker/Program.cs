using Application;
using Infrastructure;
using PaymentGateway.Worker.BackgroundTasks;
using PaymentGateway.Worker.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddRabbitMQClient("rabbitmq");
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<RabbitMqListener>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapHub<PaymentHub>("/notify-payment-hub");
app.MapGet("/", () => "Worker node running");
app.Run();