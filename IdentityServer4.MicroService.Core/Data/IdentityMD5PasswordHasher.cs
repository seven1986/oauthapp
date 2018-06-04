using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;

namespace IdentityServer4.MicroService.Data
{
    /// <summary>
    ///  used for replace Identity sha1 password haser
    /// </summary>
    public class IdentityMD5PasswordHasher : IPasswordHasher<AppUser>
    {
        public string HashPassword(AppUser user, string password) =>
            md5String(password);

        public PasswordVerificationResult VerifyHashedPassword(AppUser user, string hashedPassword, string providedPassword)
        {
            if (md5String(providedPassword).Equals(hashedPassword))
            {
                return PasswordVerificationResult.Success;
            }

            return PasswordVerificationResult.Failed;
        }

        private string md5String(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(str);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        }
    }
}
