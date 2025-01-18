using GoldenRaspberryAwards.Infrastructure.Data;
using GoldenRaspberryAwards.Api.Configuration;
using Microsoft.EntityFrameworkCore;
using GoldenRaspberryAwards.CsvDataLoader.Services;
using GoldenRaspberryAwards.Core.Interfaces;
using GoldenRaspberryAwards.Core.Interfaces.Notifications;
using GoldenRaspberryAwards.Core.Interfaces.Services;
using Asp.Versioning.ApiExplorer;
using GoldenRaspberryAwards.Core.Model.Validations;
using GoldenRaspberryAwards.Core.Model;
using GoldenRaspberryAwards.Core.Notifications;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GoldenRaspberryAwardsContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<Func<object, Movie>>(provider => (object param) =>
{
    return new Movie();
});

builder.Services.AddScoped(typeof(ICsvProcessorService<>), typeof(CsvProcessorService<>));

builder.Services.AddScoped<IEntityValidator<Movie>, MovieValidator>();

builder.Services.AddScoped<INotifier, Notifier>();

var apiSettings = new ApiSettings();
apiSettings.ConfigureServices(builder.Services, builder.Configuration, builder.Environment);

var app = builder.Build();

var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
apiSettings.ConfigurePipeline(app, app.Environment, apiVersionDescriptionProvider);

app.Run();