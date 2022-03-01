using System.ComponentModel.DataAnnotations;

namespace OAuthApp.Models
{
    public class OAuthSignInModel
    {
        [Required]
        public string scheme { get; set; }

        [Required]
        public string client_id { get; set; }

        public string scope { get; set; }

        public string nonce { get; set; }

        public string state { get; set; }

        [Required]
        public string redirect_uri { get; set; }

        // email / mobile / pwd

        public string grantType { get; set; }

        public string code { get; set; }
    }
}
