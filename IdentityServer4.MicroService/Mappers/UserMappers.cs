using AutoMapper;
using System.Collections.Generic;
using IdentityServer4.MicroService.Models.AppUsersModels;

namespace IdentityServer4.MicroService.Mappers
{
    public static class UserMappers
    {
        static UserMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserMapperProfile>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static AppUserModel ToModel(this AppUser user)
        {
            return Mapper.Map<AppUserModel>(user);
        }

        public static List<AppUserModel> ToModels(this List<AppUser> user)
        {
            return Mapper.Map<List<AppUserModel>>(user);
        }

        public static AppUser ToEntity(this AppUserModel user)
        {
            return Mapper.Map<AppUser>(user);
        }

        public static List<AppUser> ToEntities(this List<AppUserModel> user)
        {
            return Mapper.Map<List<AppUser>>(user);
        }
    }
}
