using AutoMapper;

namespace IdentityServer4.MicroService.Mappers
{
    public class UserControllerMappers : Profile
    {
        public UserControllerMappers()
        {
           
        }
    }

    public static class UserControllerMappersHelper
    {
        static UserControllerMappersHelper()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserControllerMappers>())
                .CreateMapper();
        }

        internal static IMapper Mapper { get; }
    }
}
