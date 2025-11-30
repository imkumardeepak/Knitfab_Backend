using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;

namespace AvyyanBackend.Extensions
{
    public static class SerilogExtensions
    {
        public static IServiceCollection AddSerilogServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Serilog
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            services.AddSerilog();
            return services;
        }

        public static WebApplicationBuilder ConfigureSerilog(this WebApplicationBuilder builder)
        {
            // Clear default logging providers
            builder.Logging.ClearProviders();

            // Configure Serilog
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithEnvironmentName()
                    .Enrich.WithProcessId()
                    .Enrich.WithProcessName()
                    .Enrich.WithThreadId()
                    .Enrich.WithProperty("Application", "AvyyanKnitfab")
                    .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                    .WriteTo.File(
                        path: "logs/avyyan-knitfab-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 2,
                        fileSizeLimitBytes: 10_000_000,
                        rollOnFileSizeLimit: true,
                        formatter: new JsonFormatter(),
                        restrictedToMinimumLevel: LogEventLevel.Information)
                    .WriteTo.File(
                        path: "logs/avyyan-knitfab-errors-.log",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 2,
                        fileSizeLimitBytes: 10_000_000,
                        rollOnFileSizeLimit: true,
                        restrictedToMinimumLevel: LogEventLevel.Error,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}")
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                    .MinimumLevel.Override("System", LogEventLevel.Warning);

                // Set minimum level based on environment
                if (context.HostingEnvironment.IsDevelopment())
                {
                    configuration.MinimumLevel.Debug();
                }
                else
                {
                    configuration.MinimumLevel.Information();
                }
            });

            return builder;
        }

        public static WebApplication UseSerilogRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging(options =>
            {
                // Customize the message template
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

                // Emit debug-level events instead of the defaults
                options.GetLevel = (httpContext, elapsed, ex) => ex != null
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : httpContext.Response.StatusCode > 399
                            ? LogEventLevel.Warning
                            : LogEventLevel.Information;

                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers.UserAgent.FirstOrDefault() ?? "Unknown");
                    diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");

                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserName", httpContext.User.Identity.Name ?? "Unknown");
                        diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value ??
                                                        httpContext.User.FindFirst("id")?.Value ?? "Unknown");
                    }
                };
            });

            return app;
        }
    }
}