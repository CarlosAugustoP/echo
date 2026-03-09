using EchoProject.Api.DependencyInjection;
using EchoProject.Application.Common.Password;
using EchoProject.Domain.Interfaces;
using EchoProject.Infrastructure.UnitOfWork;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger();
builder.Services.AddPostgresDatabase(builder.Configuration);
builder.Services.ConfigureBlockChain(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddRepositoriesAndUnitOfWork();    
builder.Services.AddAutoMapper
(
    cfg => { cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"]; 
        }, typeof(EchoProject.Application.AssemblyReference).Assembly
);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddAppServices(
    typeof(EchoProject.Application.AssemblyReference).Assembly);
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddValidatorsFromAssemblyContaining<EchoProject.Application.AssemblyReference>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwagger();
}
app.UseAuthentication();
app.UseAuthorization();
app.AddMiddlewares();
app.MapControllers();
app.Run();
