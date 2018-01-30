using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.RoleModels
{
    public class GetResponse
    {
        public List<AppRole> roles { get; set; }

        public int total { get; set; }
    }
}
