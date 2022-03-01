using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.ApiModels.TenantServersController
{
    public class MarketResponse
    {
        public long ID { get; set; }

        public string Tag { get; set; }

        public string ServerName { get; set; }

        public string Summary { get; set; }

        public string WebSiteUrl { get; set; }
    }
}
