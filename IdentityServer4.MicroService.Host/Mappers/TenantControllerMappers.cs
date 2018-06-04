using AutoMapper;
using IdentityServer4.MicroService.Models.Shared;
using IdentityServer4.MicroService.Tenant;
using System.Linq;

namespace IdentityServer4.MicroService.Mappers
{
    public class TenantControllerMappers : Profile
    {
        public TenantControllerMappers()
        {
            CreateMap<AppTenant, TenantPublicModel>()
                  .ForMember(x => x.claims,
                  opts => opts.MapFrom(src => src.Claims.ToDictionary(x => x.ClaimType, x => x.ClaimValue)));

            CreateMap<AppTenant, TenantPrivateModel>()
                  .ForMember(x => x.Properties,
                  opts => opts.MapFrom(src => src.Properties.ToDictionary(x => x.Key, x => x.Value)));
        }
    }

    public static class TenantControllerMappersHelper
    {
        static TenantControllerMappersHelper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<TenantControllerMappers>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static TenantPublicModel ToPublicModel(this AppTenant tenant)
        {
            return Mapper.Map<TenantPublicModel>(tenant);
        }

        public static TenantPrivateModel ToPrivateModel(this AppTenant tenant)
        {
            return Mapper.Map<TenantPrivateModel>(tenant);
        }
    }
}
