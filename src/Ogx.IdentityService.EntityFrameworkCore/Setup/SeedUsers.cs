using Ogx.Shared.Helper.Consts;

namespace Ogx.IdentityService.EntityFrameworkCore.Setup;

internal static class SeedUsers
{
    private const string DefaultPlainPassword = "Passw0rd!";

    public static List<SeedUser> Users => new()
    {
        new SeedUser
        {
            UserId = Guid.Parse("4A670F80-5592-44D3-AFD5-C8DFCA3679E4"),
            Username = DefaultRoleNames.SystemAdmin,
            PlainPassword = DefaultPlainPassword,
            GivenName = DefaultDomainNames.System,
            FamilyName = IdentityConsts.Admin,
            Email = $"{IdentityConsts.Admin}@{IdentityConsts.SolutionName}.com",
            Roles = { DefaultRoleNames.SystemAdmin },
            AvatarUrl = "/demo-hsnsoft-48x48.png"
        },
        new SeedUser
        {
            UserId = Guid.Parse("BA271C49-4FCD-4143-8A17-5B6B7452AF66"),
            Username = DefaultRoleNames.SystemUser,
            PlainPassword = DefaultPlainPassword,
            GivenName = DefaultDomainNames.System,
            FamilyName = IdentityConsts.User,
            Email = $"{IdentityConsts.User}@{IdentityConsts.SolutionName}.com",
            Roles = { DefaultRoleNames.SystemUser },
            AvatarUrl = "/demo-hsnsoft-48x48.png"
        },
        new SeedUser
        {
            UserId = Guid.Parse("ABCD12BF-09B6-4DA9-AB62-D5F1DDD5FABC"),
            Username = "hsnsh",
            PlainPassword = DefaultPlainPassword,
            GivenName = "Hasan",
            FamilyName = "SAHIN",
            Email = "hsnsh@outlook.com",
            Phone = "905335551122",
            Roles = { DefaultRoleNames.AppUser },
            AvatarUrl = "/demo-hsnsoft-48x48.png"
        },
    };
}

internal sealed class SeedUser
{
    public Guid UserId { get; set; }

    public string Username { get; set; }
    public string PlainPassword { get; set; }

    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public List<string> Roles { get; set; } = new();

    public string Lang { get; set; }
    public string AvatarUrl { get; set; }
}