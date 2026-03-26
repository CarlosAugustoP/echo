using EchoProject.BlockchainWorker;
using EchoProject.Infrastructure.DependencyInjection;
using Rebus.Config;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<TransactionValidationWorker>();

var rabbitHost = builder.Configuration["RabbitMqSettings:Host"] ?? "localhost";
var rabbitConnString = $"amqp://guest:guest@{rabbitHost}";

builder.Services.AddRebus(configure => configure
    .Transport(t => t.UseRabbitMqAsOneWayClient(rabbitConnString))
);

var host = builder.Build();
host.Run();