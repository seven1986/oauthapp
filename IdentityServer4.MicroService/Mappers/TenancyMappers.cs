using AutoMapper;
using IdentityServer4.MicroService.Models.Shared;
using IdentityServer4.MicroService.Tenant;

namespace IdentityServer4.MicroService.Mappers
{
    public static class TenancyMappers
    {
        static TenancyMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<TenancyMapperProfile>())
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
