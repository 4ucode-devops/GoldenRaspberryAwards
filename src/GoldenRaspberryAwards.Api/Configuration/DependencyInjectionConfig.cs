using FluentValidation;
using GoldenRaspberryAwards.Core.Enum;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Repositories;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Core.Notifications;
using GoldenRaspberryAwards.CsvDataLoader.Services;
using GoldenRaspberryAwards.Identity.Extensions;
using GoldenRaspberryAwards.Identity.Interfaces;
using GoldenRaspberryAwards.Identity.Services;
using GoldenRaspberryAwards.SharedServices.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GoldenRaspberryAwards.Api.Configuration;

public static class DependencyInjectionConfig
{
    public static void AddDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        RegisterManualServices(services);

        services.Scan(scan => scan
            .FromAssemblies(
                typeof(INotifier).Assembly,
                typeof(IRepository<>).Assembly,
                typeof(BaseService).Assembly,
                typeof(IValidator<>).Assembly
            )
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Service") ||
                                                         type.Name.EndsWith("Repository") ||
                                                         type.Name.EndsWith("UnitOfWork") ||
                                                         type.Name.EndsWith("Validator")))
                .AsImplementedInterfaces()
                .WithScopedLifetime()
            .AddClasses(classes => classes.Where(type => type.Name.EndsWith("Producer") ||
                                                         type.Name.EndsWith("Consumer") ||
                                                         typeof(IHostedService).IsAssignableFrom(type)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime()
        );

        services.AddTransient(provider =>
        {
            var message = "Default message";
            return new Notification(message, NotificationType.Information);
        });
    }

    private static void RegisterManualServices(IServiceCollection services)
    {
        services.TryAddScoped<INotifier, Notifier>();
        services.TryAddScoped<IAspNetUser, AspNetUser>();
        services.TryAddScoped<IAuthService, AuthService>();

        services.TryAddScoped<ICsvProcessorService<Movie>, CsvProcessorService<Movie>>();

        services.TryAddScoped<RoleManager<IdentityRole>>();
        services.AddAuthorization();
        services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }
}
