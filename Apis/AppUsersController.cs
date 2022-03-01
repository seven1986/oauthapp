using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using OAuthApp.Filters;
using Microsoft.AspNetCore.Authorization;
using OAuthApp.Tenant;
using OAuthApp.ApiModels.AppUsersController;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用用户")]
    public class AppUsersController : BaseController
    {
        private readonly AppDbContext _context;
        private readonly TokenProvider _tokenProvider;
        private readonly TenantDbContext _tenantContext;
        private readonly TenantContext _tenant;
        private readonly IMemoryCache _cache;
        private readonly static Random _random = new Random();

        public AppUsersController(AppDbContext context,
             TokenProvider tokenProvider,
            IHttpContextAccessor contextAccessor,
            TenantDbContext tenantContext,
            IMemoryCache cache)
        {
            _context = context;
            _tokenProvider = tokenProvider;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
            _tenantContext = tenantContext;
            _cache = cache;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "AppUsers")]
        [EncryptResultFilter]
        public IActionResult List(long appId,
            string userName,
            string email,
            string phone,
            string platform,
            string unionId,
            int skip, int take)
        {
            var q = _context.AppUsers.AsQueryable();

            if (appId > 0)
            {
                q = q.Where(x => x.AppID == appId);
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                q = q.Where(x => x.UserName.Contains(userName));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                q = q.Where(x => x.Email.Equals(email));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                q = q.Where(x => x.Phone.Equals(phone));
            }

            if (!string.IsNullOrWhiteSpace(platform))
            {
                q = q.Where(x => x.Platform.Equals(platform));
            }

            if (!string.IsNullOrWhiteSpace(unionId))
            {
                q = q.Where(x => x.UnionID.Equals(unionId));
            }

            var total = q.Count();
            var data = q.OrderByDescending(x => x.ID).Skip(skip).Take(take).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "AppUser")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.AppUsers
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "AppUserPut")]
        public IActionResult Put(long id, AppUser appUser)
        {
            if (id != appUser.ID)
            {
                return NotFound();
            }

            _context.Entry(appUser).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return OK(true);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "AppUserDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.AppUsers
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            _context.AppUsers.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }

        //#region 手机
        //#region 手机登录
        //[HttpPost("MobileSignIn")]
        //[SwaggerOperation(OperationId = "AppUserMobileSignIn")]
        //[AllowAnonymous]
        //public IActionResult MobileSignIn(MobileSignInRequest value)
        //{
        //    if (!ExistsApp(value.AppID))
        //    {
        //        return Error("不存在的应用");
        //    }

        //    var appUser = _context.AppUsers.FirstOrDefault(x =>
        //    x.AppID == value.AppID && x.Phone == value.Phone);

        //    if (appUser == null)
        //    {
        //        return Error("用户不存在");
        //    }

        //    var code = _cache.Get<string>("mobile_" + value.VerifyCode);

        //    if (string.IsNullOrWhiteSpace(code))
        //    {
        //        return Error("验证码错误或已过期");
        //    }

        //    var result = CreateToken(new TenantUserModel()
        //    {
        //        Avatar = appUser.Avatar,
        //        Email = appUser.Email,
        //        ID = appUser.ID,
        //        Mobile = appUser.Phone,
        //        Name = appUser.UserName,
        //        NickName = appUser.NickName,
        //        Role = UserConst.DefaultAppUserRole
        //    }, value.AppID);

        //    return OK(result);
        //} 
        //#endregion

        //#region 发送手机验证码
        //[HttpPost("SendMobileVerifyCode")]
        //[SwaggerOperation(OperationId = "AppUserSendMobileVerifyCode")]
        //[AllowAnonymous]
        //public IActionResult SendMobileVerifyCode(MobileSignInRequest value)
        //{
        //    if (!ExistsApp(value.AppID))
        //    {
        //        return Error("不存在的应用");
        //    }

        //    var appUser = _context.AppUsers.FirstOrDefault(x =>
        //    x.AppID == value.AppID && x.Phone == value.Phone);

        //    if (appUser == null)
        //    {
        //        return Error("用户不存在");
        //    }

        //    var verifyCode = _random.Next(111111, 999999);

        //    _cache.Set("mobile_" + verifyCode.ToString(),
        //        "true",
        //        new DateTimeOffset(DateTime.Now.AddMinutes(5)));

        //    return OK(true);
        //}
        //#endregion

        //#region 手机注册
        //[HttpPost("MobileSignUp")]
        //[SwaggerOperation(OperationId = "AppUserMobileSignUp")]
        //[AllowAnonymous]
        //public IActionResult MobileSignUp(MobileSignUpRequest value)
        //{
        //    if (!ExistsApp(value.AppID))
        //    {
        //        return Error("不存在的应用");
        //    }

        //    if (_context.AppUsers.Any(x => x.AppID == value.AppID && x.Phone == value.Phone))
        //    {
        //        return Error("账号已存在");
        //    }

        //    var code = _cache.Get<string>("mobile_" + value.VerifyCode);

        //    if (string.IsNullOrWhiteSpace(code))
        //    {
        //        return Error("验证码错误或已过期");
        //    }

        //    var user = CreateUser(new CreateUserModel()
        //    {
        //        AppID = value.AppID,
        //        Avatar = value.Avatar,
        //        Email = "",
        //        NickName = value.Phone,
        //        Phone = value.Phone,
        //        Platform = value.Platform,
        //        Pwd = value.Pwd,
        //        UserName = value.Phone,
        //        UnionID = "",
        //        EmailIsValid = false,
        //        PhoneIsValid = true
        //    }, 0);

        //    var result = CreateToken(user, value.AppID);

        //    return OK(result);
        //}
        //#endregion
        //#endregion

        //#region 邮箱
        //#region 邮箱注册
        //[HttpPost("EmailSignUp")]
        //[SwaggerOperation(OperationId = "AppUserEmailSignUp")]
        //[AllowAnonymous]
        //public IActionResult EmailSignUp(EmailSignUpRequest value)
        //{
        //    if (!ExistsApp(value.AppID))
        //    {
        //        return Error("不存在的应用");
        //    }

        //    if (_context.AppUsers.Any(x => x.AppID == value.AppID && x.Email == value.Email))
        //    {
        //        return Error("账号已存在");
        //    }

        //    var code = _cache.Get<string>("email_" + value.VerifyCode);

        //    if (string.IsNullOrWhiteSpace(code))
        //    {
        //        return Error("验证码错误或已过期");
        //    }

        //    var user = CreateUser(new CreateUserModel()
        //    {
        //        AppID = value.AppID,
        //        Avatar = value.Avatar,
        //        Email = value.Email,
        //        NickName = value.Email,
        //        Phone = "",
        //        Platform = value.Platform,
        //        Pwd = value.Pwd,
        //        UserName = value.Email,
        //        UnionID = "",
        //        EmailIsValid = true,
        //        PhoneIsValid = false
        //    }, 0);

        //    var result = CreateToken(user, value.AppID);

        //    return OK(result);
        //}
        //#endregion

        //#region 邮箱登录
        //[HttpPost("SignInByEmail")]
        //[SwaggerOperation(OperationId = "AppUserEmailSignIn")]
        //[AllowAnonymous]
        //public IActionResult EmailSignIn(EmailSignInRequest value)
        //{
        //    if (!ExistsApp(value.AppID))
        //    {
        //        return Error("不存在的应用");
        //    }

        //    var appUser = _context.AppUsers.FirstOrDefault(x =>
        //    x.AppID == value.AppID && x.Email == value.Email);

        //    if (appUser == null)
        //    {
        //        return Error("用户不存在");
        //    }

        //    var code = _cache.Get<string>("email_" + value.VerifyCode);

        //    if (string.IsNullOrWhiteSpace(code))
        //    {
        //        return Error("验证码错误或已过期");
        //    }

        //    var result = CreateToken(new TenantUserModel()
        //    {
        //        Avatar = appUser.Avatar,
        //        Email = appUser.Email,
        //        ID = appUser.ID,
        //        Mobile = appUser.Phone,
        //        Name = appUser.UserName,
        //        NickName = appUser.NickName,
        //        Role = UserConst.DefaultAppUserRole
        //    }, value.AppID);

        //    return OK(result);
        //} 
        //#endregion

        //#region 发送邮箱验证码
        //[HttpPost("SendEmailVerifyCode")]
        //[SwaggerOperation(OperationId = "AppUserSendEmailVerifyCode")]
        //[AllowAnonymous]
        //public IActionResult SendEmailVerifyCode(EmailSignInRequest value)
        //{
        //    if (!ExistsApp(value.AppID))
        //    {
        //        return Error("不存在的应用");
        //    }

        //    var appUser = _context.AppUsers.FirstOrDefault(x =>
        //    x.AppID == value.AppID && x.Email == value.Email);

        //    if (appUser == null)
        //    {
        //        return Error("用户不存在");
        //    }

        //    var verifyCode = _random.Next(111111, 999999);

        //    _cache.Set("email_" + verifyCode.ToString(),
        //        "true",
        //        new DateTimeOffset(DateTime.Now.AddMinutes(5)));

        //    return OK(true);
        //}
        //#endregion
        //#endregion

        #region UnionID
        #region UnionID登录
        [HttpPost("UnionIDSignIn")]
        [SwaggerOperation(OperationId = "AppUserUnionIDSignIn")]
        [AllowAnonymous]
        public IActionResult UnionIDSignIn(UnionIDSignInRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            AppUser appUser = null;

            try
            {
                appUser = _context.AppUsers.FirstOrDefault(x =>
                 x.AppID == value.AppID &&
                 x.UnionID == value.UnionID &&
                 x.Platform == value.Platform);
            }
            catch (ArgumentException ex)
            {
                return Error(ex.Message);
            }

            if (appUser == null)
            {
                return Error("用户不存在");
            }

            var result = CreateToken(new TenantUserModel()
            {
                Avatar = appUser.Avatar,
                Email = appUser.Email,
                ID = appUser.ID,
                Mobile = appUser.Phone,
                Name = appUser.UserName,
                NickName = appUser.NickName,
                Role = UserConst.DefaultAppUserRole
            }, value.AppID);

            return OK(result);
        }
        #endregion

        #region UnionID注册
        [HttpPost("UnionIDSignUp")]
        [SwaggerOperation(OperationId = "AppUserUnionIDSignUp")]
        [AllowAnonymous]
        public IActionResult UnionIDSignUp(UnionIDSignUpRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            if (_context.AppUsers.Any(x => x.AppID == value.AppID &&
            x.Platform == value.Platform && x.UnionID == value.UnionID))
            {
                return Error("账号已存在");
            }

            var user = CreateUser(new CreateUserModel()
            {
                AppID = value.AppID,
                Avatar = value.Avatar,
                Email = "",
                NickName = value.UnionID,
                Phone = "",
                Platform = value.Platform,
                Pwd = value.Pwd,
                UserName = "",
                UnionID = value.UnionID,
                EmailIsValid = false,
                PhoneIsValid = false
            }, 0);

            var result = CreateToken(user, value.AppID);

            return OK(result);
        }
        #endregion
        #endregion

        #region 账号密码
        #region 登录
        [HttpPost("SignIn")]
        [SwaggerOperation(OperationId = "AppUserSignIn")]
        [AllowAnonymous]
        public IActionResult SignIn(SignInRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            AppUser appUser = null;

            try
            {
                appUser = _context.AppUsers.FirstOrDefault(x =>
                 x.AppID == value.AppID &&
                 x.UserName == value.UserName);
            }
            catch (ArgumentException ex)
            {
                return Error(ex.Message);
            }

            if (appUser == null)
            {
                return Error("用户不存在");
            }

            else if (!appUser.Pwd.Equals(value.Pwd))
            {
                return Error("密码错误");
            }

            var result = CreateToken(new TenantUserModel()
            {
                Avatar = appUser.Avatar,
                Email = appUser.Email,
                ID = appUser.ID,
                Mobile = appUser.Phone,
                Name = appUser.UserName,
                NickName = appUser.NickName,
                Role = UserConst.DefaultAppUserRole
            }, value.AppID);

            return OK(result);
        }
        #endregion

        #region 注册
        [HttpPost("SignUp")]
        [SwaggerOperation(OperationId = "AppUserSignUp")]
        [AllowAnonymous]
        public IActionResult SignUp(SignUpRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            if (_context.AppUsers.Any(x => x.AppID == value.AppID && x.UserName == value.UserName))
            {
                return Error("账号已存在");
            }

            var user = CreateUser(new CreateUserModel()
            {
                AppID = value.AppID,
                Avatar = value.Avatar,
                Email = "",
                NickName = value.NickName,
                Phone = "",
                Platform = value.Platform,
                Pwd = value.Pwd,
                UserName = value.UserName,
                UnionID = "",
                EmailIsValid = false,
                PhoneIsValid = false
            }, 0);

            var result = CreateToken(user, value.AppID);

            return OK(result);
        }
        #endregion
        #endregion

        #region 微信小程序
        #region 网页登录
        [HttpPost("QRCodeSignIn")]
        [SwaggerOperation(OperationId = "AppUserQRCodeSignIn")]
        [AllowAnonymous]
        public IActionResult QRCodeSignIn(QRCodeSignInRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            var item = _tenantContext.QRCodeSignIns.FirstOrDefault(x =>
            x.ChannelCode == ChannelCodes.App &&
            x.ChannelAppID == value.AppID.ToString() &&
            x.SignInKey == value.SignInKey);

            if (item == null)
            {
                return Error("二维码失效请刷新");
            }

            if (item.Status.Equals(QRCodeSignInStatus.PreSignIn))
            {
                return Error("请扫码");
            }

            if (item.Status.Equals(QRCodeSignInStatus.Scaned))
            {
                return Error("登陆中...");
            }

            var appUser = _context.AppUsers.FirstOrDefault(x => x.ID == item.AppUserID);

            if (appUser == null)
            {
                return Error("用户不存在");
            }

            var result = CreateToken(new TenantUserModel()
            {
                Avatar = appUser.Avatar,
                Email = appUser.Email,
                ID = appUser.ID,
                Mobile = appUser.Phone,
                Name = appUser.UserName,
                NickName = appUser.NickName,
                Role = UserConst.DefaultAppUserRole
            }, value.AppID);

            return OK(result);
        }
        #endregion

        #region 注册
        [HttpPost("QRCodeSignUp")]
        [SwaggerOperation(OperationId = "AppUserQRCodeSignUp")]
        [AllowAnonymous]
        public IActionResult QRCodeSignUp(QRCodeSignUpRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            var item = _tenantContext.QRCodeSignIns.Where(x => x.TenantID == _tenant.Id &&
            x.ChannelCode == ChannelCodes.App &&
            x.ChannelAppID == value.AppID.ToString() &&
            x.SignInKey == value.SignInKey).FirstOrDefault();

            if (item == null || item.Status.Equals(QRCodeSignInStatus.PreSignIn))
            {
                return Error("二维码已失效，请刷新重试");
            }

            var appUser = _context.AppUsers.Where(x => x.AppID == value.AppID &&
            x.Platform == value.Platform &&
            x.UnionID == value.UnionID).FirstOrDefault();

            TenantUserModel user = null;

            if (appUser == null)
            {
               user = CreateUser(new CreateUserModel()
                {
                    AppID = value.AppID,
                    Avatar = value.Avatar,
                    Email = value.Email,
                    NickName = value.NickName,
                    Phone = value.Phone,
                    Platform = value.Platform,
                    Pwd = value.UnionID,
                    UserName = value.UserName,
                    UnionID = value.UnionID,
                    EmailIsValid = value.EmailIsValid,
                    PhoneIsValid = value.PhoneIsValid
                }, 0);

                item.AppUserID = user.ID;
                item.Status = QRCodeSignInStatus.OK;
                item.LastUpdate = DateTime.Now;
                _tenantContext.SaveChanges();
            }
            else
            {
                user = new TenantUserModel()
                {
                    ID = appUser.ID,
                    Name = appUser.UserName,
                    NickName = appUser.NickName ?? "",
                    Email = appUser.Email ?? "",
                    Mobile = appUser.Phone ?? "",
                    Avatar = appUser.Avatar ?? "",
                    Role = UserConst.DefaultAppUserRole
                };
            }

            var result = CreateToken(user, value.AppID);

            item.AppUserID = user.ID;
            item.Status = QRCodeSignInStatus.OK;
            item.LastUpdate = DateTime.Now;
            _tenantContext.SaveChanges();

            return OK(result);
        }
        #endregion

        #region 扫码通知
        [HttpPost("QRCodeScan")]
        [SwaggerOperation(OperationId = "AppUserQRCodeScan")]
        [AllowAnonymous]
        public IActionResult QRCodeScan(QRCodeScanRequest value)
        {
            var appItem = _context.Apps.Find(value.AppID);

            if (appItem == null)
            {
                return Error("不存在的应用");
            }

            var item = _tenantContext.QRCodeSignIns.Where(x => x.TenantID == _tenant.Id &&
            x.ChannelCode == ChannelCodes.App &&
            x.ChannelAppID == value.AppID.ToString() &&
            x.SignInKey == value.SignInKey).FirstOrDefault();

            if (item == null || !item.Status.Equals(QRCodeSignInStatus.PreSignIn))
            {
                return Error("二维码已失效，请刷新重试");
            }

            item.Status = QRCodeSignInStatus.Scaned;
            item.LastUpdate = DateTime.Now;
            _tenantContext.SaveChanges();

            return OK(new
            {
                AppID = appItem.ID,
                Name = appItem.Name,
                Logo = appItem.Logo,
                Website = appItem.Website,
                Description = appItem.Description,
                Tags = appItem.Tags
            });
        }
        #endregion

        #region 预登陆
        [HttpPost("QRCodePreSignIn")]
        [SwaggerOperation(OperationId = "AppUserQRCodePreSignIn")]
        [AllowAnonymous]
        public IActionResult QRCodePreSignIn(QRCodePreSignInRequest value)
        {
            if (!ExistsApp(value.AppID))
            {
                return Error("不存在的应用");
            }

            var Props = _context.PropertySettings.Where(x => x.ChannelCode == ChannelCodes.App &&
            x.ChannelAppId == value.AppID).ToList();

            var ClientID = Props.Where(x => x.Name.Equals(PropKeyConst.WechatMiniPClientID))
                .Select(x => x.Value).FirstOrDefault();

            var ClientSecret = Props.Where(x => x.Name.Equals(PropKeyConst.WechatMiniPClientSecret))
                .Select(x => x.Value).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(ClientID)|| string.IsNullOrWhiteSpace(ClientSecret))
            {
                return Error("应用未配置小程序参数");
            }

            var qrcodeSignInItem = new QRCodeSignIn()
            {
                ChannelCode = ChannelCodes.App,
                ChannelAppID = value.AppID.ToString(),
                TenantID = _tenant.Id
            };

            _tenantContext.QRCodeSignIns.Add(qrcodeSignInItem);

            _tenantContext.SaveChanges();

            return OK(qrcodeSignInItem.SignInKey);
        }
        #endregion
        #endregion

        #region 获取个人资料
        [HttpGet("Profile")]
        [SwaggerOperation(OperationId = "AppUserProfile")]
        public IActionResult Profile()
        {
            var appUser = _context.AppUsers.FirstOrDefault(x => x.ID == UserID);

            if (appUser == null)
            {
                return Error("不存在的用户");
            }

            return OK(new
            {
                appUser.AppID,
                appUser.Platform,
                appUser.UnionID,
                appUser.Phone,
                appUser.CreateDate,
                appUser.UserName,
                appUser.PhoneIsValid,
                appUser.Data,
                appUser.Email,
                appUser.EmailIsValid,
                appUser.LastUpdate,
                appUser.NickName,
                appUser.ID,
                appUser.Avatar,
                Role = UserConst.DefaultAppUserRole
            });
        }
        #endregion

        #region 更新个人资料
        [HttpPut("Profile")]
        [SwaggerOperation(OperationId = "AppUserUpdateProfile")]
        public IActionResult UpdateProfile(UpdateProfileRequest value)
        {
            var appUser = _context.AppUsers.FirstOrDefault(x => x.ID == UserID);

            if (appUser == null)
            {
                return Error("不存在的用户");
            }

            if (!string.IsNullOrWhiteSpace(value.NickName) && !appUser.NickName.Equals(value.NickName))
            {
                appUser.NickName = value.NickName;
            }

            if (!string.IsNullOrWhiteSpace(value.Avatar) && !appUser.Avatar.Equals(value.Avatar))
            {
                appUser.Avatar = value.Avatar;
            }

            if (appUser.Data == null || (!string.IsNullOrWhiteSpace(value.Data) && !appUser.Data.Equals(value.Data)))
            {
                appUser.Data = value.Data;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return OK(true);
        } 
        #endregion

        #region helper
        private bool ExistsApp(long appId)
        {
            return _context.Apps.Any(x => x.ID == appId && !x.IsDelete);
        }
        private TokenModel CreateToken(TenantUserModel user, long appId)
        {
            var claims = _tenant.CreateClaims(user, appId.ToString());

            var token = _tokenProvider.CreateToken(items =>
            {
                items.AddRange(claims);
            });

            return token;
        }
        private TenantUserModel CreateUser(CreateUserModel m,long userId)
        {
            var appUser = new AppUser()
            {
                Platform = m.Platform,
                UnionID = m.UnionID,
                UserID = userId,
                UserName = m.UserName,
                AppID = m.AppID,
                NickName = m.NickName,
                Avatar = m.Avatar,
                Phone = m.Phone,
                PhoneIsValid = m.PhoneIsValid,
                Email = m.Email,
                EmailIsValid = m.EmailIsValid,
                Pwd = m.Pwd
            };

            _context.AppUsers.Add(appUser);

            _context.SaveChanges();

            var userModel = new TenantUserModel()
            {
                ID = appUser.ID,
                Name = m.UserName,
                NickName = m.NickName ?? "",
                Email = m.Email ?? "",
                Mobile = m.Phone ?? "",
                Avatar = m.Avatar ?? "",
                Role = UserConst.DefaultAppUserRole
            };

            return userModel;
        }
        #endregion
    }
}
