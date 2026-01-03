using Ogx.IdentityService.Application.Contracts.AppRoleDomain.Services;
using Ogx.IdentityService.Application.Contracts.AppUserDomain.Services;
using Ogx.IdentityService.Application.Services;
using Ogx.Shared.Contracts.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Ogx.IdentityService.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddServiceApplicationConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(typeof(ApplicationAutoMapperProfile));

        services.AddSingleton<IServicePermissionProvider, ApplicationPermissionProvider>();

        // Must be Scoped or Transient => Cannot consume any scoped service
        services.AddScoped<IAppUserAppService, AppUserAppService>();
        services.AddScoped<IAppRoleAppService, AppRoleAppService>();

        return services;
    }
}