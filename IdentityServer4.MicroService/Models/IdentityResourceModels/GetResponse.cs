using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.IdentityResourceModels
{
    public class GetResponse
    {
        public IList<IdentityResource> resources { get; set; }

        public int total { get; set; }
    }
}
