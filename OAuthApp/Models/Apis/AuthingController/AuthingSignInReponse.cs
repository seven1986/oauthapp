namespace OAuthApp.Models.Apis.AuthingController
{
   public class AuthingSignInReponse
    {
        public bool Succeeded { get; set; }
     
        public bool IsLockedOut { get; set; }
     
        public bool IsNotAllowed { get; set; }
     
        public bool RequiresTwoFactor { get; set; }

        public string Message { get; set; }
    }
}
