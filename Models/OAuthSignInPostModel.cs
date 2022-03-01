using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.Models
{
    public class OAuthSignInPostModel: OAuthSignInModel
    {
        public string userName { get; set; }

        public string Pwd { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }
    }
}
