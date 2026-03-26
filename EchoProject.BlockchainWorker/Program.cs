using EchoProject.BlockchainWorker;
using EchoProject.Infrastructure.DependencyInjection;
using Rebus.Config;

var builder = Host.CreateApplicationBuilder(args);

// 1. Carrega as configurações de infra (Banco, etc)
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Registra o serviço que valida a blockchain
builder.Services.AddHostedService<TransactionValidationWorker>();

// 3. Configura o Rebus usando o Host que vem do Docker Compose
var rabbitHost = builder.Configuration["RabbitMqSettings:Host"] ?? "localhost";
var rabbitConnString = $"amqp://guest:guest@{rabbitHost}";

builder.Services.AddRebus(configure => configure
    .Transport(t => t.UseRabbitMqAsOneWayClient(rabbitConnString))
);

var host = builder.Build();
host.Run();