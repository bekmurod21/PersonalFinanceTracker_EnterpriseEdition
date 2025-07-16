using Serilog;
using PersonalFinanceTracker_EnterpriseEdition.Api.Extensions;
using PersonalFinanceTracker_EnterpriseEdition.Api.Extensions.Configurations;
using PersonalFinanceTracker_EnterpriseEdition.Api.Middlewares;
using PersonalFinanceTracker_EnterpriseEdition.Application;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure;
using PersonalFinanceTracker_EnterpriseEdition.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Environment'ga qarab log papkasini aniqlash
var environment = builder.Environment.EnvironmentName;
var logPath = environment == "Production" 
    ? "/var/log/personalfinancetracker" 
    : Path.Combine(Directory.GetCurrentDirectory(), "Logs");

// Log papkasini yaratish
if (!Directory.Exists(logPath))
{
    Directory.CreateDirectory(logPath);
}

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCustomServices(builder.Configuration);
builder.ConfigureCORSPolicy();
builder.Services.AddHttpContextAccessor();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"] ?? "localhost:6379";
});

// Serilog sozlash
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Environment", environment)
    .Enrich.WithProperty("Application", "PersonalFinanceTracker_EnterpriseEdition")
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);

var app = builder.Build();

ServiceExtension.EnsureExportsDirectoryExists();

app.ApplyMigration();
app.InitAccessor();

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
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

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHealthChecks("/health");
app.UseHealthChecks("/health/live");
app.UseHealthChecks("/health/ready");
app.MapControllers();

logger.Information("PersonalFinanceTracker Enterprise Edition is starting up...");

app.Run();