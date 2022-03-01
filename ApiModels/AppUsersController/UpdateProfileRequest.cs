using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class UpdateProfileRequest
    {
        public string Avatar { get; set; }
        public string Data { get; set; }
        public string NickName { get; set; }
    }
}
