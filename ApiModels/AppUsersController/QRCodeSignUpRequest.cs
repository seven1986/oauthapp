namespace OAuthApp.ApiModels.AppUsersController
{
    public class QRCodeSignUpRequest
    {
        public long AppID { get; set; }

        public string SignInKey { get; set; }
        public string UnionID { get; set; }
        public string Avatar { get; set; }
        public string Platform { get; set; }
        public string NickName { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public bool PhoneIsValid { get; set; } = false;
        public string Email { get; set; }
        public bool EmailIsValid { get; set; } = false;
    }
}
