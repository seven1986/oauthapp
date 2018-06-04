using AutoMapper;
using IdentityServer4.MicroService.Models.Apis.ApiResourceController;
using IdentityServer4.MicroService.Services;

namespace IdentityServer4.MicroService.Mappers
{
    public class ApiResourceControllerMappers : Profile
    {
        public ApiResourceControllerMappers()
        {
            CreateMap<AzureApiManagementApiEntity, ApiResourceVersionsResponse>();
        }
    }

    public static class ApiResourceControllerMappersHelper
    {
        static ApiResourceControllerMappersHelper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceControllerMappers>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }

        public static ApiResourceVersionsResponse ToModel(this AzureApiManagementApiEntity tenant)
        {
            return Mapper.Map<ApiResourceVersionsResponse>(tenant);
        }
    }
}
