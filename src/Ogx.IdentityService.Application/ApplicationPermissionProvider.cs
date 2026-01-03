using Ogx.Shared.Contracts.Cache;
using Ogx.Shared.Contracts.Cache.ServicePermissions;

namespace Ogx.IdentityService.Application;

public sealed class ApplicationPermissionProvider : IServicePermissionProvider
{
    public Task<List<string>> GetServicePermissionKeysAsync() => Task.FromResult(IdentityServicePermissions.GetAll().ToList());
}