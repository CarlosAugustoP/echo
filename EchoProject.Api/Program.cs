using System.IdentityModel.Tokens.Jwt;
using EchoProject.Api.DependencyInjection;
using EchoProject.Application.DependencyInjection;
using EchoProject.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAutoMapper
(
    cfg => { cfg.LicenseKey = builder.Configuration["AutoMapper:LicenseKey"]; 
        }, typeof(EchoProject.Application.AssemblyReference).Assembly
);
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});
builder.Services.AddCorsPolicy();
builder.Services.ConfigureSwagger();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddControllers();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwagger();
}
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
app.UseCors("DefaultCorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
await app.SubscribeRebusEventsAsync();
app.AddMiddlewares();
app.MapControllers();
app.Run();
