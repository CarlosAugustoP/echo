using EchoProject.Api.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureSwagger();
builder.Services.AddPostgresDatabase(builder.Configuration);
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.AddSwagger();
}

app.Run();
