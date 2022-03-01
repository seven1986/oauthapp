using OAuthApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OAuthApp.Tenant
{
    public class TokenProvider
    {
        private readonly JwtSettings _jwtSettingsAccesser;

        public ClaimsIdentity UserClaims { get; set; }

        #region User
        private TenantUserModel _User;
        public TenantUserModel User
        {
            get
            {
                if (_User == null)
                {
                    //var TenantID = UserClaims.FindFirst(TenantClaimTypes.TenantId).Value;
                    //var TenantName = UserClaims.FindFirst(TenantClaimTypes.TenantName).Value;
                    var ID = UserClaims.FindFirst(ClaimTypes.NameIdentifier).Value;
                    var Name = UserClaims.FindFirst(ClaimTypes.Name).Value;
                    var Role = UserClaims.FindFirst(ClaimTypes.Role).Value;
                    var NickName = UserClaims.FindFirst(ClaimTypes.GivenName).Value;
                    var Avatar = UserClaims.FindFirst(TenantClaimTypes.Picture).Value;
                    var Email = UserClaims.FindFirst(ClaimTypes.Email).Value;
                    var Mobile = UserClaims.FindFirst(TenantClaimTypes.Mobile).Value;
                    //var UnionID = UserClaims.FindFirst(TenantClaimTypes.UnionID).Value;
                    //var ClientId = UserClaims.FindFirst(TenantClaimTypes.ClientId).Value;

                    _User = new TenantUserModel()
                    {
                        Avatar = Avatar,
                        Email = Email,
                        ID = long.Parse(ID),
                        Mobile = Mobile,
                        Name = Name,
                        NickName = NickName,
                        Role = Role
                    };
                }

                return _User;
            }
        }
        #endregion

        public TokenProvider(
            IOptions<JwtSettings> jwtSettingsAccesser,
            IHttpContextAccessor contextAccessor
            )
        {
            UserClaims = contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
            _jwtSettingsAccesser = jwtSettingsAccesser.Value;
        }

        public long GetTimeStamp(DateTime authTime)
        {
            var ts = authTime - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public TokenModel CreateToken(Action<List<Claim>> _TokenClaims = null, int expireDay = 1)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, DateTime.UtcNow.Ticks.ToString()));

            if (_TokenClaims != null)
            {
                _TokenClaims.Invoke(claims);
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.UTF8.GetBytes(_jwtSettingsAccesser.SecretKey);

            var authTime = DateTime.UtcNow;

            var expiresAt = authTime.AddDays(expireDay);

            var tokenDescripor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature),
                Audience = _jwtSettingsAccesser.Audience,
                Issuer = _jwtSettingsAccesser.Issuer,
            };

            var token = tokenHandler.CreateToken(tokenDescripor);

            var tokenString = tokenHandler.WriteToken(token);

            var result = new TokenModel()
            {
                access_token = tokenString,
                token_type = JwtBearerDefaults.AuthenticationScheme,
                expires_in = GetTimeStamp(authTime)
            };

            return result;
        }
    }
}
