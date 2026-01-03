using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ogx.IdentityService.Application;
using Ogx.IdentityService.Domain.AppRoleDomain.Entities;
using Ogx.IdentityService.Domain.AppUserDomain.Entities;
using Ogx.IdentityService.Domain.Localization;
using Ogx.IdentityService.EntityFrameworkCore;
using Ogx.IdentityService.EntityFrameworkCore.Context;
using Ogx.Shared.Contracts.Cache.ServicePermissions;
using Ogx.Shared.Contracts.Events;
using Ogx.Shared.Hosting;
using Ogx.Shared.Hosting.Microservices;
using Ogx.Shared.Hosting.Microservices.Middlewares;
using Ogx.Shared.Hosting.Middlewares;
using HsnSoft.Base.AspNetCore.Localization;
using HsnSoft.Base.Data;
using HsnSoft.Base.Tracing;
using Microsoft.AspNetCore.Identity;

namespace Ogx.IdentityService;

public sealed class Startup
{
    private IConfiguration Configuration { get; }
    private IWebHostEnvironment WebHostEnvironment { get; }

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        WebHostEnvironment = environment;
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
        services.ConfigureMicroserviceHost(Configuration, typeof(Startup))
            .AddJwtServerAuthentication(Configuration, WebHostEnvironment, "audience-service-identity")
            .AddCustomAuthorization(IdentityServicePermissions.GetAll())
            .AddEventBus(Configuration, typeof(EventHandlersAssemblyMarker).Assembly)
            .AddMicroserviceUserTenantChecker()
            .AddHostingHealthChecks(Configuration, "identity", checkRedis: true, checkBroker: true,
                checkPostgresql: true, postgresqlConnectionName: EfCoreDbProperties.ConnectionStringName)
            .AddServiceApplicationConfiguration(Configuration)
            .AddServiceEfCoreDatabaseConfiguration(Configuration, !WebHostEnvironment.IsHostProduction());

        // override DefaultBasicDataSeeder
        services.AddTransient<IBasicDataSeeder, EfCoreSeederService>();

        services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.AllowedUserNameCharacters = "abcçdefghiıjklmnoöpqrsştuüvwxyzABCÇDEFGHIİJKLMNOÖPQRSŞTUÜVWXYZ0123456789-._@+'#!/^%{}*";
            })
            .AddEntityFrameworkStores<IdentityAppDbContext>()
            .AddDefaultTokenProviders();

        if (!WebHostEnvironment.IsHostProduction())
        {
            SwaggerConfigurationHelper.ConfigureWithBearer(services,
                "Please enter a valid token. Token audiences contains audience-service-identity",
                $"{ApplicationIdentifier.AppName} API");
        }

        var container = new ContainerBuilder();
        container.Populate(services);

        return new AutofacServiceProvider(container.Build());
    }

    public void Configure(IApplicationBuilder app, IHostApplicationLifetime hostApplicationLifetime)
    {
        if (!WebHostEnvironment.IsHostProduction())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", $"{ApplicationIdentifier.AppName} API");
            });
        }

        app.UseLocalization(typeof(IdentityServiceResource));

        /*Middleware*/
        app.UseMiddleware<RequestResponseLoggerMiddleware>();
        app.UseMiddleware<BaseLocalizationMiddleware>();
        app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseUserTenantChecker();

        app.UseEndpoints(endpoints =>
        {
            if (!WebHostEnvironment.IsHostProduction())
            {
                endpoints.MapDefaultControllerRoute();
            }
            else
            {
                endpoints.MapControllers();

                var buildNumber = Environment.GetEnvironmentVariable("BUILD_NUMBER");
                var appVersion = !string.IsNullOrWhiteSpace(buildNumber) ? $"v1.0.{buildNumber}" : "v1.0.0";
                endpoints.MapGet("/", () => $"OGX {ApplicationIdentifier.AppName} | {ApplicationIdentifier.AppId} | {WebHostEnvironment.EnvironmentName} | {appVersion}");
            }
        });
        app.UseHostingHealthChecks();

        // Subscribe all event handlers
        app.UseEventBus(typeof(EventHandlersAssemblyMarker).Assembly, new Dictionary<string, ushort>()
        {
            { nameof(CachePermissionGrantsChangedEto), 1 }
        });

        hostApplicationLifetime.ApplicationStopping.Register(OnShutdown);
    }

    private static void OnShutdown() => Console.WriteLine("Stopping web host ({0})...", ApplicationIdentifier.AppName);
}