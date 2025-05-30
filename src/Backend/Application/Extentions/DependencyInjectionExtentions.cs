using Application.Users.Commands;
using Application.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extentions;

public static class DependencyInjectionExtentions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssemblyContaining<LoginUserCommand>();
        });

        return services;
    }

    public static IServiceCollection AddValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<LoginUserCommandValidator>();

        return services;
    }
}
