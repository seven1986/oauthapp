using AutoMapper;
using IdentityServer4.MicroService.Models.AppUsersModels;
using System.Linq;

namespace IdentityServer4.MicroService.Mappers
{
    public class UserMapperProfile: Profile
    {
        public UserMapperProfile()
        {
            CreateMap<AppUser, AppUserModel>()
                .ForMember(x => x.Claims,
                opts => opts.MapFrom(src => src.Claims.Select(x => new Claim()
                {
                    Id = x.Id,
                    ClaimValue = x.ClaimValue,
                    ClaimType = x.ClaimType,
                })))

                .ForMember(x => x.Logins,
                opts => opts.MapFrom(src => src.Logins.Select(x => new Login()
                {
                    LoginProvider = x.LoginProvider,
                    ProviderKey = x.ProviderKey
                })))

                .ForMember(x => x.Roles,
                opts => opts.MapFrom(src => src.Roles.Select(x => new Role()
                {
                    Id = x.RoleId
                })))
                 .ForMember(x => x.Files,
                opts => opts.MapFrom(src => src.Files.Select(x => new File()
                {
                    Id = x.Id,
                    FileType = x.FileType,
                    Files = x.Files
                })));
        }
    }

}
