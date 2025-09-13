using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace ProjectGamma.Configuration.Security;

public static class JwtResourceServerExtensions
{
    public static IServiceCollection AddAlphaJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool requireHttpsMetadata = false,
        Action<JwtBearerOptions>? configure = null)
    {
        var issuer   = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key      = configuration["Jwt:Key"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience) ||
            string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException(
                "Jwt settings are missing. Please set Jwt:Issuer, Jwt:Audience, and Jwt:Key in configuration.");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = requireHttpsMetadata;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                configure?.Invoke(options);
            });

        return services;
    }
}
