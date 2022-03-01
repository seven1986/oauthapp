using OAuthApp.Data;

namespace OAuthApp.ApiModels.UserController
{
    public class AppUserItem : AppUser
    {
        public string AppName { get; set; }
        public string AppLogo { get; set; }
    }
}
