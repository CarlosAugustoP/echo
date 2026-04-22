using EchoProject.BlockchainWorker;
using EchoProject.Infrastructure.DependencyInjection;
using Rebus.Config;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHostedService<TransactionValidationWorker>();

var rabbitHost = builder.Configuration["RabbitMqSettings:Host"] ?? "localhost";
var rabbitUser = builder.Configuration["RabbitMqSettings:Username"] ?? "guest";
var rabbitPass = builder.Configuration["RabbitMqSettings:Password"] ?? "guest";
var rabbitVHost = builder.Configuration["RabbitMqSettings:VirtualHost"] ?? "/";
var rabbitConnString = $"amqp://{rabbitUser}:{rabbitPass}@{rabbitHost}/{Uri.EscapeDataString(rabbitVHost)}";
Console.WriteLine("[BC WORKER] try to connect to "+ rabbitConnString);
builder.Services.AddRebus(configure => configure
    .Transport(t => t.UseRabbitMqAsOneWayClient(rabbitConnString))
);

var host = builder.Build();
host.Run();
