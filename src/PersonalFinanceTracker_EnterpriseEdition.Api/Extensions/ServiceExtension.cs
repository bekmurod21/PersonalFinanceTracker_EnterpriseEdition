using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonalFinanceTracker_EnterpriseEdition.Api.Models;
using PersonalFinanceTracker_EnterpriseEdition.Domain.Helpers;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Extensions;

public static class ServiceExtension
{
     public static void AddCustomServices(this IServiceCollection services,IConfiguration configuration)
    {
        services.AddControllers().AddJsonOptions(options =>
                                                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(
                                                new ConfigureApiUrlName()));
        })
        .AddJsonOptions(x =>
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
        
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "application/javascript",
                "application/json",
                "text/css",
                "text/html"
            });
        });

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("V1", new OpenApiInfo()
            {
                Version = "V1",
                Title = "PersonalFinanceTracker_EnterpriseEdition",
                Description = "Provides public administration."
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Bearer Authentication",
                Type = SecuritySchemeType.Http
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

        services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"))
            .AddPolicy("UserOnly", policy => policy.RequireRole("User"));

        services.AddSingleton<IHealthCheckPublisher, HealthCheckPublisher>();

        EnvironmentHelper.DatabaseUrl = configuration.GetConnectionString("DefaultConnection");

        services.AddHealthChecks()
                .AddNpgSql(EnvironmentHelper.DatabaseUrl);
    }

    public static void EnsureExportsDirectoryExists()
    {
        try
        {
            var exportsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "exports");
            if (!Directory.Exists(exportsPath))
            {
                Directory.CreateDirectory(exportsPath);
                Log.Information("Created exports directory: {ExportsPath}", exportsPath);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to create exports directory");
        }
    }
}