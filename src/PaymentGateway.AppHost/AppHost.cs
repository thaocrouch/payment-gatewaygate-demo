using PaymentGateway.AppHost;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// SQL Server
var sqlPassword = builder.AddParameter("SqlPassword", true);
var sql = builder
    .AddSqlServer("sql")
    .WithImageTag("2022-latest")
    .WithPassword(sqlPassword)
    .AddDatabase("PaymentGateway");

// RabbitMQ
var rabbitmqUserName = builder.AddParameter("RabbitmqUserName", true);
var rabbitmqPassword = builder.AddParameter("RabbitmqPassword", true);
var rabbitmqQueue = builder.AddParameter("RabbitmqQueue", true);
var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", rabbitmqUserName, rabbitmqPassword)
    .WithManagementPlugin();

// WebApi
var api = builder.AddProject<PaymentGateway_Api>("webapi")
    .WithReference(sql)
    .WithReference(rabbitmq)
    .WithEnvironment("ConnectionStrings:PaymentGateway", sql.Resource.ConnectionStringExpression)
    .WithEnvironment("RabbitMQ:Queue", rabbitmqQueue.Resource.Value)
    .WithExternalHttpEndpoints()
    .WithConfigurationSection(builder.Configuration.GetSection("Email"))
    .WithConfigurationSection(builder.Configuration.GetSection("InternalSystem"))
    .WaitFor(sql)
    .WaitFor(rabbitmq);

var notifyWorker = builder.AddProject<PaymentGateway_Worker>("worker")
    .WithReference(rabbitmq)
    .WithExternalHttpEndpoints()
    .WithEnvironment("ConnectionStrings:PaymentGateway", sql.Resource.ConnectionStringExpression)
    .WithConfigurationSection(builder.Configuration.GetSection("Email"))
    .WithConfigurationSection(builder.Configuration.GetSection("InternalSystem"))
    .WaitFor(rabbitmq)
    .WaitFor(api);

var webclient = builder.AddProject<WebClient>("webclient")
    .WithEnvironment(ctx =>
    {
        var notifyWorkerEndpoint = notifyWorker.GetEndpoint("http");
        ctx.EnvironmentVariables["Hub__BaseUrl"] = notifyWorkerEndpoint?.Url ?? "";
        ctx.EnvironmentVariables["Hub__Path"] = "notify-payment-hub";

        var apiEndpoint = api.GetEndpoint("http");
        ctx.EnvironmentVariables["BackendUrl"] = apiEndpoint?.Url ?? "";
    })
    .WaitFor(notifyWorker);

builder.Build().Run();