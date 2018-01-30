using AutoMapper;
using IdentityServer4.MicroService.Models.AppTenantModels;
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

        public static AppTenantPublicModel ToPublicModel(this AppTenant tenant)
        {
            return Mapper.Map<AppTenantPublicModel>(tenant);
        }

        public static AppTenantPrivateModel ToPrivateModel(this AppTenant tenant)
        {
            return Mapper.Map<AppTenantPrivateModel>(tenant);
        }
    }
}
