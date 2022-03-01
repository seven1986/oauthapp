using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.ApiModels.AppUsersController
{
    public class CreateUserModel
    {
        public string Platform { get; set; }
        public string UnionID { get; set; }
        public string UserName { get; set; }
        public string Pwd { get; set; }
        public long AppID { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Phone { get; set; }
        public bool PhoneIsValid { get; set; }
        public string Email { get; set; }
        public bool EmailIsValid { get; set; }
    }
}
