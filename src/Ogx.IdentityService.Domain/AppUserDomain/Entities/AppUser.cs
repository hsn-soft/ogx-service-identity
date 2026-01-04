using System.Globalization;
using System.Text;
using Ogx.IdentityService.Domain.AppUserDomain.Consts;
using Ogx.Shared.Localization;
using HsnSoft.Base;
using Microsoft.AspNetCore.Identity;

namespace Ogx.IdentityService.Domain.AppUserDomain.Entities;

public sealed class AppUser : IdentityUser<Guid>, ISoftDelete
{
    public bool IsDeleted { get; internal set; }

    public bool IsSystemUser { get; private set; }

    public string Name { get; private set; }

    public string Surname { get; private set; }

    public string DefaultLanguage { get; internal set; }

    public string AvatarSuffixUrl { get; internal set; }


    private AppUser()
    {
        UserName = string.Empty;
        Email = string.Empty;
    }

    internal AppUser(Guid id,
        string userName, string email, string phone,
        string name,
        string surname,
        string defaultLanguage,
        string avatarSuffixUrl,
        bool isSystemUser = false
    ) : this()
    {
        Id = id;
        IsSystemUser = isSystemUser;
        SetDefaultLanguage(defaultLanguage);

        SetUserName(userName);
        SetEmail(email);
        SetPhone(phone);
        SetName(name);
        SetSurname(surname);
        AvatarSuffixUrl = avatarSuffixUrl;
    }

    internal void SetDefaultLanguage(string defaultLanguage)
    {
        DefaultLanguage = LocalizedModelValidator.Length(defaultLanguage, $"{nameof(AppUser)}:{nameof(DefaultLanguage)}", AppUserConsts.DefaultLanguageMaxLength);

        var acceptableLanguages = new[] { "en", "ru", "tr" };
        if (!acceptableLanguages.Contains((DefaultLanguage ?? "").ToLower(new CultureInfo("en-US"))))
        {
            DefaultLanguage = "en";
        }
    }

    internal void SetUserName(string username)
    {
        var checkUserName = LocalizedModelValidator.NotNullOrWhiteSpace(username, $"{nameof(AppUser)}:{nameof(UserName)}", AppUserConsts.UserNameMaxLength);

        UserName = string.Join("", checkUserName.ToLower(new CultureInfo("en-US")).Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        NormalizedUserName = string.Join("", UserName.ToUpper().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }

    internal void SetEmail(string email)
    {
        var checkEmail = LocalizedModelValidator.NotNullOrWhiteSpace(email, $"{nameof(AppUser)}:{nameof(Email)}", AppUserConsts.EmailMaxLength);

        Email = string.Join("", checkEmail.ToLower(new CultureInfo("en-US")).Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));

        NormalizedEmail = string.Join("", Email.ToUpper().Normalize(NormalizationForm.FormD)
            .Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
    }

    internal void SetPhone(string phone)
    {
        PhoneNumber = LocalizedModelValidator.Length(phone, $"{nameof(AppUser)}:{nameof(PhoneNumber)}", AppUserConsts.PhoneNumberMaxLength);
    }

    internal void SetName(string name)
    {
        Name = LocalizedModelValidator.Length(name, $"{nameof(AppUser)}:{nameof(Name)}", AppUserConsts.NameMaxLength);
    }

    internal void SetSurname(string surname)
    {
        Surname = LocalizedModelValidator.Length(surname, $"{nameof(AppUser)}:{nameof(Surname)}", AppUserConsts.SurnameMaxLength);
    }
}