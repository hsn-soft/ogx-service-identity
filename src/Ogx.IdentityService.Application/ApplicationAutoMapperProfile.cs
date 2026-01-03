using AutoMapper;
using Ogx.IdentityService.Application.Contracts.AppRoleDomain.Dtos;
using Ogx.IdentityService.Application.Contracts.AppUserDomain.Dtos;
using Ogx.IdentityService.Domain.AppRoleDomain.Entities;
using Ogx.IdentityService.Domain.AppUserDomain.Entities;
using Ogx.Shared.Helper.Utils;

namespace Ogx.IdentityService.Application;

public class ApplicationAutoMapperProfile : Profile
{
    public ApplicationAutoMapperProfile()
    {
        CreateMap<AppUser, AppUserDto>()
            .ForMember(dest => dest.UserName,
                opt => opt.MapFrom(source => StringOperations.SplitFirstValue(source.UserName, "#")))
            .ForMember(dest => dest.Email,
                opt => opt.MapFrom(source => StringOperations.SplitFirstValue(source.Email, "#")));

        CreateMap<AppRole, AppRoleDto>()
            .ForMember(dest => dest.Name,
                opt => opt.MapFrom(source => StringOperations.SplitFirstValue(source.Name, "#")));
    }
}