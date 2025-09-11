using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using ProjectGamma.Application.Services;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ProjectGamma.Configuration.Services;

public static class StartupConfiguration
{
    public static IServiceCollection AddStartupServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

        services.AddSingleton<IAirlineService, AirlineService>();
        services.AddSingleton<IAirportService, AirportService>();

        return services;
    }

    private sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        : IConfigureOptions<SwaggerGenOptions>
    {
        public void Configure(SwaggerGenOptions options)
        {
            foreach (var desc in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    desc.GroupName,
                    new OpenApiInfo
                    {
                        Title = $"ProjectGamma API v{desc.ApiVersion}",
                        Version = desc.ApiVersion.ToString(),
                        Description = $"ProjectGamma API version {desc.ApiVersion}"
                    }
                );
            }
        }
    }
}
