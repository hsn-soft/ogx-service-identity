using JetBrains.Annotations;

namespace Ogx.IdentityService.Application.Contracts.AppRoleDomain.Dtos;

public sealed class AppRoleDto
{
    public Guid Id { get; set; }

    [CanBeNull]
    public string Name { get; set; }

    public bool IsDefault { get; set; }
    public bool IsStatic { get; set; }
    public bool IsPublic { get; set; }
}