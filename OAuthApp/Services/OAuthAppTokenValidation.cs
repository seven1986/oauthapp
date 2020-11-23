using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace OAuthApp.Services
{
    public class OAuthAppTokenValidation: TokenValidationParameters
    {
        public OAuthAppTokenValidation()
        {
            //IssuerSigningKeyValidator = _IssuerSigningKeyValidator;
            AudienceValidator = _AudienceValidator;
            IssuerValidator = _IssuerValidator;
            //LifetimeValidator = _LifetimeValidator;
            //SignatureValidator = _SignatureValidator;
            //TokenReplayValidator = _TokenReplayValidator;
        }

        //private bool _TokenReplayValidator(DateTime? expirationTime, string securityToken, TokenValidationParameters validationParameters)
        //{

        //    return true;
        //}

        //private SecurityToken _SignatureValidator(string token, TokenValidationParameters validationParameters)
        //{
        //    new JwtSecurityToken()

        //    throw new NotImplementedException();
        //}

        //private bool _LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        //{
        //    return true;
        //}
        private string _IssuerValidator(string issuer, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            Console.WriteLine($"_IssuerValidator issuer:{issuer}");

            return issuer;
        }

        private bool _AudienceValidator(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            var txt = string.Join("，", audiences.ToList());

            Console.WriteLine($"_AudienceValidator audiences:{txt}");

            return true;
        }

        //bool _IssuerSigningKeyValidator(SecurityKey securityKey,
        //    SecurityToken securityToken, TokenValidationParameters validationParameters)
        //{
        //    return true;
        //}
    }
}
