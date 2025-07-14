using Serilog;
using PersonalFinanceTracker_EnterpriseEdition.Api.Extensions;
using PersonalFinanceTracker_EnterpriseEdition.Api.Extensions.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Api.Middlewares;
using PersonalFinanceTracker_EnterpriseEdition.Application;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomServices();
builder.ConfigureCORSPolicy();
builder.Services.AddHttpContextAccessor();
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();

var logger = new LoggerConfiguration()
    .ReadFrom
    .Configuration(builder.Configuration)
    .Enrich
    .FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);

var app = builder.Build();
app.ApplyMigration();
app.InitAccessor();
if (app.Environment.IsDevelopment()|| app.Environment.IsProduction())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/V1/swagger.json", "PersonalFinanceTracker_EnterpriseEdition");
        options.ConfigObject.AdditionalItems["persistAuthorization"] = "true";
    });
}
app.UseCors("AllowAll");
app.UseCors("AllowHeaders");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHealthChecks("/health");
app.MapControllers();

app.Run();