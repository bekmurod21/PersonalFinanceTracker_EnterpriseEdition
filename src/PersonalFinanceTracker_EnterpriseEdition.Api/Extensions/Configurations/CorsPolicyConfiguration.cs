using Microsoft.Net.Http.Headers;

namespace PersonalFinanceTracker_EnterpriseEdition.Api.Extensions.Configurations;

public static class CorsPolicyConfiguration
{
    public static void ConfigureCORSPolicy(this WebApplicationBuilder builder)
    {
        var allowOrigins = "AllowOrigins";
        builder.Services.AddCors(option =>
        {
            option.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });
            option.AddPolicy("AllowHeaders", builder =>
            {
                builder.AllowAnyHeader();
                builder.WithOrigins(allowOrigins)
                    .WithHeaders(HeaderNames.ContentType, HeaderNames.Server, HeaderNames.AccessControlAllowHeaders,
                                 HeaderNames.AccessControlExposeHeaders,HeaderNames.Allow,
                                 "x-custom-header","Pagination","x-time", "x-path", "x-record-in-use",
                                 HeaderNames.ContentDisposition);
            });
        });
    }
}