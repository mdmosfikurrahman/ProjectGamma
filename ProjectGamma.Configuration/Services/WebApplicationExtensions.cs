using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ProjectGamma.Configuration.Services;

public static class WebApplicationExtensions
{
    public static WebApplication UseStartupPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        $"ProjectGamma API v{description.ApiVersion}"
                    );
                }
            });
        }

        app.UseHttpsRedirection();

        app.MapControllers();

        return app;
    }
}