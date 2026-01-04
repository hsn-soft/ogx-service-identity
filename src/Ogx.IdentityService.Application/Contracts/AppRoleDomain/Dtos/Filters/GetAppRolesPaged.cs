using HsnSoft.Base.Application.Dtos;
using JetBrains.Annotations;

namespace Ogx.IdentityService.Application.Contracts.AppRoleDomain.Dtos.Filters;

public sealed class GetAppRolesPaged : PagedDataRequestDto
{
    [CanBeNull]
    public string Name { get; set; } = null;

    public bool? IsDefault { get; set; } = null;
    public bool? IsStatic { get; set; } = null;
    public bool? IsPublic { get; set; } = null;
}