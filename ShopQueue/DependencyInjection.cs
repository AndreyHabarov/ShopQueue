using FluentValidation;
using FluentValidation.AspNetCore;
using ShopQueue.Validators;

namespace ShopQueue;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<JoinQueueRequestValidator>();
        services.AddFluentValidationAutoValidation();

        return services;
    }
}
