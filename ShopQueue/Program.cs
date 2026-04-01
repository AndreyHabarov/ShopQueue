using Scalar.AspNetCore;
using ShopQueue;
using ShopQueue.Application;
using ShopQueue.Infrastructure;
using ShopQueue.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();