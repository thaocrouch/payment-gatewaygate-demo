var builder = DistributedApplication.CreateBuilder(args);

// SQL Server
var sqlPassword = builder.AddParameter("SqlPassword", secret: true);
var sql = builder
    .AddSqlServer("sql")
    .WithImageTag("2022-latest")
    .WithPassword(sqlPassword)
    .AddDatabase("PaymentGateway");

// RabbitMQ
var rabbitmqUserName = builder.AddParameter("RabbitmqUserName", secret: true);
var rabbitmqPassword = builder.AddParameter("RabbitmqPassword", secret: true);
var rabbitmqQueue = builder.AddParameter("RabbitmqQueue", secret: true);
var rabbitmq = builder
    .AddRabbitMQ("rabbitmq", rabbitmqUserName, rabbitmqPassword)
    .WithManagementPlugin();

// WebApi
var api = builder.AddProject<Projects.PaymentGateway_Api>("webapi")
    .WithReference(sql)
    .WithReference(rabbitmq)
    .WithEnvironment("ConnectionStrings:PaymentGateway", sql.Resource.ConnectionStringExpression)
    .WithEnvironment("RabbitMq:Queue", rabbitmqQueue.Resource.Value)
    .WithExternalHttpEndpoints()
    .WaitFor(sql)
    .WaitFor(rabbitmq);

var notifyWorker = builder.AddProject<Projects.PaymentGateway_Worker>("worker")
    .WithReference(rabbitmq)
    .WithExternalHttpEndpoints()
    .WaitFor(rabbitmq)
    .WaitFor(api);

var webclient = builder.AddProject<Projects.WebClient>("webclient")
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
