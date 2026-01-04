using Ogx.Shared.Helper.Consts;

namespace Ogx.IdentityService.EntityFrameworkCore.Setup;

internal static class SeedRoles
{
    public static List<SeedRole> Roles => new()
    {
        new SeedRole
        {
            RoleId = Guid.Parse("E2F3F225-A555-49D9-8E8B-CDE113C18C4C"),
            Name = DefaultRoleNames.SystemAdmin,
            IsPublic = false,
        },
        new SeedRole
        {
            RoleId = Guid.Parse("8E19FFE7-5671-44A6-84B3-BD447A75FAEC"), Name = DefaultRoleNames.SystemUser,
        },
        new SeedRole
        {
            RoleId = Guid.Parse("ABCDEF6D-370F-4FDC-9BCA-0330FF0DFABC"),
            Name = DefaultRoleNames.AppUser,
            IsPublic = false,
            IsDefault = true,
        },
    };
}

internal sealed class SeedRole
{
    public Guid RoleId { get; set; }
    public string Name { get; set; }

    public bool IsStatic { get; set; } = true; // can't delete
    public bool IsPublic { get; set; } = true; // view commercial role list
    public bool IsDefault { get; set; } = false; // register screen user
}