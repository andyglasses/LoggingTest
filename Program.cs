using LoggingTest;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Http;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Host.UseSerilog((ctx, services, lc) =>
{
    lc.Enrich.WithExceptionDetails()
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(ctx.Configuration);

    var serilogHttpSinkUrl = ctx.Configuration.GetValue<string>("SerilogHttpSinkURL");
    if (!string.IsNullOrEmpty(serilogHttpSinkUrl))
    {
        lc.WriteTo.Http(
            requestUri: serilogHttpSinkUrl,
            queueLimitBytes: 10 * ByteSize.MB);
    }

    if (ctx.HostingEnvironment.IsDevelopment())
        lc.WriteTo.Console();
    else
        lc.WriteTo.ApplicationInsights(services.GetRequiredService<TelemetryConfiguration>(),
            TelemetryConverter.Traces);
    
});

builder.Services.AddScoped<DemoConnector>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck<SubApiHealthCheck>("API", HealthStatus.Unhealthy, new [] {"detailed"});

builder.Services.AddApplicationInsightsTelemetry();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();
/*
 app.UseSerilogRequestLogging(options =>
{
    options.GetLevel = (HttpContext ctx, double _, Exception ex) =>
    {
        return ex != null
            ? LogEventLevel.Error
            : ctx.Response.StatusCode > 499
                ? LogEventLevel.Error
                : IsHealthCheckEndpoint(ctx) // Not an error, check if it was a health check
                    ? LogEventLevel.Verbose // Was a health check, use Verbose
                    : LogEventLevel.Information;
    };
});
*/



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = (check) => check.Tags.Contains("ready") && !check.Tags.Contains("detailed")
});

app.MapHealthChecks("/health/detailed", new HealthCheckOptions
{
    ResponseWriter = (HttpContext httpContext, HealthReport result) =>
    {
        var obj = new
        {
            OverallStatus = result.Status.ToString(),
            TotalChecksDuration = result.TotalDuration.TotalSeconds.ToString("0:0:00", CultureInfo.InvariantCulture),
            Checks = result.Entries
                .Select(e =>
                    new
                    {
                        e.Key,
                        e.Value.Description,
                        e.Value.Duration,
                        e.Value.Status,
                        Error = e.Value.Exception?.Message,
                        e.Value.Data
                    })
                .ToList()
        };
        return httpContext.Response.WriteAsJsonAsync(obj);
    },
    AllowCachingResponses = false
});

app.MapControllers();

app.Run();



static bool IsHealthCheckEndpoint(HttpContext ctx)
{
    var endpoint = ctx.GetEndpoint();
    if (endpoint is object) // same as !(endpoint is null)
    {
        return string.Equals(
            endpoint.DisplayName,
            "Health checks",
            StringComparison.Ordinal);
    }
    // No endpoint, so not a health check endpoint
    return false;
}