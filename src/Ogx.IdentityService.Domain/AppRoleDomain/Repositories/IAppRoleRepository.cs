using System.Linq.Expressions;
using Ogx.IdentityService.Domain.AppRoleDomain.Entities;
using JetBrains.Annotations;

namespace Ogx.IdentityService.Domain.AppRoleDomain.Repositories;

public interface IAppRoleRepository
{
    Task<List<AppRole>> GetPagedListWithFiltersAsync(
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        int skipCount = 0,
        CancellationToken cancellationToken = default
    );

    Task<long> GetCountWithFiltersAsync(
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppRole>> GetFilterListAsync(
        [CanBeNull] string name = null,
        bool? isDefault = null,
        bool? isStatic = null,
        bool? isPublic = null,
        [CanBeNull] string sorting = null,
        CancellationToken cancellationToken = default
    );

    Task<List<AppRole>> GetSearchListAsync(
        [CanBeNull] string searchText = null,
        [CanBeNull] string sorting = null,
        int maxResultCount = int.MaxValue,
        CancellationToken cancellationToken = default
    );

    //add default default functions because dbContext is not BaseDbContext
    [ItemCanBeNull]
    Task<AppRole> FindWithIdAsync(Guid id);

    [ItemCanBeNull]
    Task<AppRole> FindAsync(Expression<Func<AppRole, bool>> predicate);

    Task<AppRole> CreateAsync(
        [NotNull] string name,
        bool isDefault,
        bool isStatic,
        bool isPublic);

    Task<AppRole> CreateAsync(Guid id,
        [NotNull] string name,
        bool isDefault,
        bool isStatic,
        bool isPublic);


    Task<AppRole> UpdateAsync(Guid id,
        [NotNull] string name,
        bool isDefault,
        bool isStatic,
        bool isPublic);

    Task DeleteAsync(Guid id);
}