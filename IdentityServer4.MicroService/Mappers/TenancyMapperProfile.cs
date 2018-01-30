using AutoMapper;
using System.Linq;
using IdentityServer4.MicroService.Models.AppTenantModels;
using IdentityServer4.MicroService.Tenant;

namespace IdentityServer4.MicroService.Mappers
{
    public class TenancyMapperProfile: Profile
    {
        public TenancyMapperProfile()
        {
            CreateMap<AppTenant, AppTenantPublicModel>()
                  .ForMember(x => x.claims,
                  opts => opts.MapFrom(src => src.Claims.ToDictionary(x => x.ClaimType, x => x.ClaimValue)));

            CreateMap<AppTenant, AppTenantPrivateModel>()
                  .ForMember(x => x.properties,
                  opts => opts.MapFrom(src => src.Properties.ToDictionary(x => x.Key, x => x.Value)));
        }
    }
}
