using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.AppUsersModels
{
    public class GetResponse
    {
        public List<AppUser> users { get; set; }

        public Dictionary<long, string> roles { get; set; }

        public int total { get; set; }
    }

    public class RoleResponse
    {
        public List<AppRole> roles { get; set; }

        public int total { get; set; }
    }
}
