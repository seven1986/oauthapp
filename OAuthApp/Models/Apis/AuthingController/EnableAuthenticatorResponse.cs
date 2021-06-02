using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthApp.Models.Apis.AuthingController
{
   public class EnableAuthenticatorResponse
    {
        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }
    }
}
