using EchoProject.BlockchainWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<TransactionValidationWorker>();

var host = builder.Build();
host.Run();
