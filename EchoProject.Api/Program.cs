using EchoProject.Api.DependencyInjection;
using EchoProject.Application.Common.Password;
using EchoProject.Domain.Interfaces;
using EchoProject.Infrastructure.UnitOfWork;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger();
builder.Services.AddPostgresDatabase(builder.Configuration);
builder.Services.ConfigureBlockChain(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddRepositoriesAndUnitOfWork();    
builder.Services.AddAutoMapper(cgp => {}, typeof(EchoProject.Application.AssemblyReference).Assembly);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddAppServices(
    typeof(EchoProject.Application.AssemblyReference).Assembly);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwagger();
}

app.MapControllers();
app.Run();
