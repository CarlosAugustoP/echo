using EchoProject.Api.DependencyInjection;
using EchoProject.Domain.Interfaces;
using EchoProject.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger();
builder.Services.AddPostgresDatabase(builder.Configuration);
builder.Services.ConfigureBlockChain(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddAppServices(
    typeof(EchoProject.Application.AssemblyReference).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwagger();
}

app.MapControllers();
app.Run();
