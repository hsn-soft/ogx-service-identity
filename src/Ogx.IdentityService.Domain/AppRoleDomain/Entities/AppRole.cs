using System.Globalization;
using System.Text;
using Ogx.IdentityService.Domain.AppRoleDomain.Consts;
using Ogx.Shared.Localization;
using HsnSoft.Base;
using Microsoft.AspNetCore.Identity;

namespace Ogx.IdentityService.Domain.AppRoleDomain.Entities;

public sealed class AppRole : IdentityRole<Guid>, ISoftDelete
{
    public bool IsDeleted { get; internal set; }

    public bool IsDefault { get; internal set; }
    public bool IsStatic { get; internal set; }
    public bool IsPublic { get; internal set; }

    private AppRole()
    {
        Name = string.Empty;
    }

    internal AppRole(Guid id,
        string name,
        bool isDefault = false,
        bool isStatic = false,
        bool isPublic = false
    ) : this()
    {
        Id = id;

        SetName(name);
        IsDefault = isDefault;
        IsStatic = isStatic;
        IsPublic = isPublic;
    }

    internal void SetName(string name)
    {
        var checkRoleName = LocalizedModelValidator.NotNullOrWhiteSpace(name, $"{nameof(AppRole)}:{nameof(Name)}", AppRoleConsts.NameMaxLength);

        Name = string.Join("", checkRoleName.ToLower(new CultureInfo("en-US")).Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        NormalizedName = string.Join("", Name.ToUpper().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }
}