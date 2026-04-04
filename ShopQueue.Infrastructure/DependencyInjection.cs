using HealthChecks.NpgSql;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShopQueue.Application.Repositories;
using ShopQueue.Application.Services;
using ShopQueue.Infrastructure.Consumers;
using ShopQueue.Infrastructure.Persistence;
using ShopQueue.Infrastructure.Repositories;
using ShopQueue.Infrastructure.Services;

namespace ShopQueue.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IShopRepository, ShopRepository>();
        services.AddScoped<IQueueRepository, QueueRepository>();
        services.AddScoped<IQueueEntryRepository, QueueEntryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IShopService, ShopService>();
        services.AddScoped<IQueueService, QueueService>();
        services.AddScoped<ICustomerService, CustomerService>();

        services.AddMassTransit(x =>
        {
            x.AddConsumer<ClientJoinedQueueConsumer>();
            x.AddConsumer<ClientCalledConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration["RabbitMq:Host"], "/", h =>
                {
                    h.Username(configuration["RabbitMq:Username"]!);
                    h.Password(configuration["RabbitMq:Password"]!);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection")!,
                name: "postgresql",
                tags: ["ready"]);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}