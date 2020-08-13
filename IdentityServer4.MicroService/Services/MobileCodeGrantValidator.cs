using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.MicroService.Data;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Services
{
    public class MobileCodeGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "mobile_code";

        private readonly ITokenValidator _validator;

        private readonly UserDbContext _db;

        private readonly RedisService _redis;

        private readonly IdentityServer4MicroServiceOptions _ismsOptions;

        // 用户管理SDK
        private readonly UserManager<AppUser> _userManager;

        public MobileCodeGrantValidator(ITokenValidator validator,
            UserDbContext db,
            RedisService redis,
            UserManager<AppUser> userManager,
            IdentityServer4MicroServiceOptions ismsOptions)
        {
            _validator = validator;
            _db = db;
            _redis = redis;
            _userManager = userManager;
            _ismsOptions = ismsOptions;
        }

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var country_code = context.Request.Raw.Get("country_code");

            var mobile = context.Request.Raw.Get("mobile");

            var mobile_code = context.Request.Raw.Get("mobile_code");

            if (string.IsNullOrWhiteSpace(country_code) ||
                string.IsNullOrWhiteSpace(mobile) ||
                string.IsNullOrWhiteSpace(mobile_code))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);

                return;
            }

            #region 校验手机验证码
            var PhoneNumberVerifyCodeKey = UserControllerKeys.VerifyCode_Phone + mobile + ":" + mobile_code;

            if (await _redis.KeyExistsAsync(PhoneNumberVerifyCodeKey) == false)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Verify Code Error");

                return;
            }

            await _redis.RemoveAsync(PhoneNumberVerifyCodeKey);
            #endregion

            var SubId = await _db.Users.Where(x => x.PhoneNumber == mobile && x.CountryCode == country_code)
                .Select(x => x.Id).FirstOrDefaultAsync();

            if (SubId == 0)
            {
                var User = new AppUser
                {
                    UserName = mobile,
                    PhoneNumber = mobile,
                    PhoneNumberConfirmed = true,
                    CreateDate = DateTime.UtcNow,
                    LastUpdateTime = DateTime.UtcNow,
                    EmailConfirmed = false,
                    CountryCode = country_code
                };

                var roleIds = _db.Roles.Where(x => x.Name.Equals(DefaultRoles.User))
                        .Select(x => x.Id).ToList();

                //var tenantIds = tenantDbContext.Tenants.Select(x => x.Id).ToList();

                var result = await AppUserService.CreateUser(1,
                    _userManager,
                    _db,
                    User,
                    roleIds,
                    $"{AppConstant.MicroServiceName}.all", new List<long>() { 1 });

                if (!result.Succeeded)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, 
                        JsonConvert.SerializeObject(result.Errors));

                    return;
                }

                SubId = User.Id;
            }

            context.Result = new GrantValidationResult(
                subject: SubId.ToString(),
                authenticationMethod: GrantType);

            return;
        }
    }

}
