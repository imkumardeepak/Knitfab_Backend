using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AvyyanBackend.Data;
using AvyyanBackend.Interfaces;
using AvyyanBackend.Repositories;
using AvyyanBackend.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using AvyyanBackend.WebSockets;
using TallyERPWebApi.Service;
using Microsoft.AspNetCore.Http;

namespace AvyyanBackend.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }

        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDispatchPlanningRepository, DispatchPlanningRepository>();
            services.AddScoped<ISalesOrderWebRepository, SalesOrderWebRepository>();
            services.AddScoped<ISlitLineRepository, SlitLineRepository>();

            return services;
        }

        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            // Add your business service registrations here
            services.AddScoped<IMachineManagerService, MachineManagerService>();
            services.AddScoped<IFabricStructureService, FabricStructureService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IYarnTypeService, YarnTypeService>();
            services.AddScoped<ISalesOrderWebService, SalesOrderWebService>();
            services.AddScoped<ITapeColorService, TapeColorService>();
            services.AddScoped<IShiftService, ShiftService>();
            services.AddScoped<IStorageCaptureService, StorageCaptureService>();
            services.AddScoped<ITransportService, TransportService>();
            services.AddScoped<ICourierService, CourierService>();
            services.AddScoped<ISlitLineService, SlitLineService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<DataSeedService>();

            // Dispatch Planning services
            services.AddScoped<DispatchPlanningService>();

            // Register HttpClient for Tally services
            services.AddHttpClient<TallyService>();
            services.AddHttpClient<PostTallyService>();
    
            // Register IHttpClientFactory
            services.AddHttpClient();
    
            // Register IHttpContextAccessor
            services.AddHttpContextAccessor();
    
            // Register Tally services
            services.AddSingleton<TallyService>();
            services.AddSingleton<PostTallyService>();
            // services.AddHostedService<TallyBackgroundService>();

            // Register Log Cleanup service
            services.AddHostedService<LogCleanupService>();

            // Register SignalR notification service
            services.AddScoped<ISignalRNotificationService, SignalRNotificationService>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Secret"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret is not configured");
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ClockSkew = TimeSpan.Zero
                };
                
                // We have to add this to inform SignalR about the user
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            (path.StartsWithSegments("/notificationhub") || path.StartsWithSegments("/chathub")))
                        {
                            // Read the token out of the query string
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        public static IServiceCollection AddValidationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            return services;
        }

        public static IServiceCollection AddAutoMapperServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program));
            return services;
        }

        public static IServiceCollection AddCorsServices(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder
                        .SetIsOriginAllowed(origin => true) // Allow any origin
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials(); // Required for SignalR
                });
            });

            return services;
        }
    }
}