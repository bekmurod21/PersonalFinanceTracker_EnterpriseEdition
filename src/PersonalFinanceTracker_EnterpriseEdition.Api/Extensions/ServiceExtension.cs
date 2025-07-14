using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using PersonalFinanceTracker_EnterpriseEdition.Api.Models;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Extensions;

public static class ServiceExtension
{
     public static void AddCustomServices(this IServiceCollection services)
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
    }
}