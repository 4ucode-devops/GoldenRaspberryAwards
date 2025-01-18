using Asp.Versioning.ApiExplorer;
using GoldenRaspberryAwards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace GoldenRaspberryAwards.Api.Configuration;

public class ApiSettings
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration));
        }

        configuration = BuildConfiguration(environment);
        services.AddSingleton(configuration);

        ConfigureDatabase(services, configuration);
        ConfigureControllers(services);
        ConfigureAdditionalServices(services, configuration);
    }

    private static IConfiguration BuildConfiguration(IWebHostEnvironment environment)
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<GoldenRaspberryAwardsContext>(options =>
            options.UseSqlite("Data Source=GoldenRaspberryAwards.db"));
    }

    private static void ConfigureControllers(IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
    }

    private static void ConfigureAdditionalServices(IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureAutomapper();
        services.AddDependencyInjection(configuration);
        services.AddApiVersioningConfiguration();
        services.AddSwaggerConfig();
        services.AddHttpContextAccessor();
        //services.AddIdentityConfig(configuration);
        services.AddAuthorization();
        services.AddAuthentication();
    }

    public void ConfigurePipeline(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseSwaggerConfig(provider);
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
