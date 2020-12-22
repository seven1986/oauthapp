using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace OAuthApp.Models.Apis.AuthingController
{
  public class MyExternalLoginsResponse
    {
        public List<UserLoginInfo> Logins { get; set; }

        public List<string> OtherLogins { get; set; }
    }
}
