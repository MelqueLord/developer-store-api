using Ambev.DeveloperEvaluation.Application;
using Ambev.DeveloperEvaluation.Common.Security;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.IoC.ModuleInitializers;

public class ApplicationModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        RegisterApplicationValidators(builder.Services);
    }

    private static void RegisterApplicationValidators(IServiceCollection services)
    {
        var validatorType = typeof(IValidator<>);
        var validators = typeof(ApplicationLayer).Assembly
            .GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .SelectMany(type => type.GetInterfaces()
                .Where(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == validatorType)
                .Select(interfaceType => new { InterfaceType = interfaceType, ImplementationType = type }));

        foreach (var validator in validators)
            services.AddTransient(validator.InterfaceType, validator.ImplementationType);
    }
}
