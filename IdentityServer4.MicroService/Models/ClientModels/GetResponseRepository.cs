using IdentityServer4.EntityFramework.Entities;
using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.ClientModels
{
    public class GetResponse
    {
        public IList<Client> clients { get; set; }

        public int total { get; set; }
    }

    /// <summary>
    /// query model
    /// </summary>
    public class ClientQuery
    {
        public string ClientID { get; set; }

        public string ClientName { get; set; }
    }
}
