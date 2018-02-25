using AutoMapper;
using System.Linq;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Models.Shared;

namespace IdentityServer4.MicroService.Mappers
{
    public class TenancyMapperProfile: Profile
    {
        public TenancyMapperProfile()
        {
            CreateMap<AppTenant, TenantPublicModel>()
                  .ForMember(x => x.claims,
                  opts => opts.MapFrom(src => src.Claims.ToDictionary(x => x.ClaimType, x => x.ClaimValue)));

            CreateMap<AppTenant, TenantPrivateModel>()
                  .ForMember(x => x.Properties,
                  opts => opts.MapFrom(src => src.Properties.ToDictionary(x => x.Key, x => x.Value)));
        }
    }
}
