using System.Linq.Expressions;
using HsnSoft.Base.Domain.Models;
using JetBrains.Annotations;
using Ogx.IdentityService.Domain.AppUserDomain.Entities;

namespace Ogx.IdentityService.Domain.AppUserDomain.Repositories;

public interface IAppUserRepository
{
    Task<PagedQueryResult<AppUser>> GetPageListAsync(
        PagedQueryOptions<AppUser> options,
        List<Guid> roleIds = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppUser>> GetListAsync(
        ListQueryOptions<AppUser> options,
        CancellationToken cancellationToken = default
    );



    Task<List<AppUser>> GetSearchListAsync(
        [CanBeNull] string searchText = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );

    Task<List<string>> GetUserRolesAsync(AppUser currentUser);

    //add default default functions because dbContext is not BaseDbContext
    [ItemCanBeNull]
    Task<AppUser> FindWithIdAsync(Guid id);

    [ItemCanBeNull]
    Task<AppUser> FindAsync(Expression<Func<AppUser, bool>> predicate);

    Task<AppUser> CreateAsync(
        [NotNull] string userName,
        [NotNull] string email,
        string phone,
        string name,
        string surname,
        string defaultLanguage,
        string avatarSuffixUrl,
        bool isSystemUser = false,
        ICollection<string> roles = null,
        string plainPassword = null
    );

    Task<AppUser> CreateAsync(Guid id,
        [NotNull] string userName,
        [NotNull] string email,
        string phone,
        string name,
        string surname,
        string defaultLanguage,
        string avatarSuffixUrl,
        bool isSystemUser = false,
        ICollection<string> roles = null,
        string plainPassword = null
    );

    Task<AppUser> UpdateAsync(Guid id,
        string userName,
        string email,
        string phone,
        string name,
        string surname,
        string defaultLanguage,
        ICollection<string> roles = null);

    Task DeleteAsync(Guid id);
}