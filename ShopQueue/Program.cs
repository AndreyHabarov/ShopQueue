using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using ShopQueue.Application.Services;
using ShopQueue.Infrastructure.Consumers;
using ShopQueue.Infrastructure.Persistence;
using ShopQueue.Infrastructure.Services;
using ShopQueue.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IShopService, ShopService>();
builder.Services.AddScoped<IQueueService, QueueService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ClientJoinedQueueConsumer>();
    x.AddConsumer<ClientCalledConsumer>();

    x.UsingRabbitMq((context, configuration) =>
    {
        configuration.Host("localhost", "/", h =>
        {
            h.Username("shopqueue");
            h.Password("shopqueue");
        });

        configuration.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.MapOpenApi();
app.MapScalarApiReference();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();