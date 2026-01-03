using HsnSoft.Base.Application.Dtos;
using Ogx.IdentityService.Application.Contracts.AppRoleDomain.Dtos;
using Ogx.IdentityService.Application.Contracts.AppRoleDomain.Dtos.Filters;
using Ogx.IdentityService.Application.Contracts.AppRoleDomain.Dtos.Submits;

namespace Ogx.IdentityService.Application.Contracts.AppRoleDomain.Services;

public interface IAppRoleAppService
{
    Task<AppRoleDto> GetAsync(Guid id);

    Task<PagedDataResultDto<AppRoleDto>> GetPagedListAsync(GetAppRolesPaged pagedInput);
    Task<List<AppRoleDto>> GetFilterListAsync(GetAppRolesFilter filterInput);
    Task<List<AppRoleDto>> GetSearchListAsync(GetAppRolesSearch searchInput);

    Task<AppRoleDto> CreateAsync(AppRoleCreateDto input);

    Task UpdateAsync(AppRoleUpdateDto input);

    Task DeleteAsync(Guid id);
}