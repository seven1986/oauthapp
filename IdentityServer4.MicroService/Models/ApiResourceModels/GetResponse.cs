using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.ApiResourceModels
{
    public class GetResponse
    {
        public IList<ApiResource> apis { get; set; }

        public int total { get; set; }
    }
}
