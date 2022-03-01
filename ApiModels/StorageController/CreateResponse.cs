using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.ApiModels.StorageController
{
    public class CreateResponse
    {
        public int RowCounts { get; set; }
        public int Used { get; set; }
        public int UnUsed { get; set; }
        public int Total { get; set; }
    }
}
