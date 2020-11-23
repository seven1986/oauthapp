using OAuthApp.Data;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthApp.Services
{

    public class OpenIdOAuthGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "openid_oauth";
        private readonly UserDbContext _db;

        public OpenIdOAuthGrantValidator(
          UserDbContext db)
        {
            _db = db;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var provider = context.Request.Raw.Get("provider");

            var providerKey = context.Request.Raw.Get("providerKey");

            if (string.IsNullOrWhiteSpace(provider) ||
                string.IsNullOrWhiteSpace(providerKey))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var SubId = await _db.UserLogins.Where(x => x.LoginProvider.Equals(provider) && x.ProviderKey.Equals(providerKey))
                .Select(x => x.UserId).FirstOrDefaultAsync();

            if (SubId == 0)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "No record has found");

                return;
            }

            context.Result = new GrantValidationResult(
                subject: SubId.ToString(),
                authenticationMethod: GrantType);

            return;
        }
    }
}
