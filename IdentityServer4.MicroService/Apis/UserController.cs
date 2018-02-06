using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Codes;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Mappers;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.AppUsersModels;
using IdentityServer4.MicroService.Services;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    // user 根据 tenantId 来获取列表、或详情、增删改

    [Route("User")]
    public class UserController : BasicController
    {
        #region Services
        //Database
        readonly ApplicationDbContext db;
        // 短信
        readonly ISmsSender sms;
        // 邮件
        readonly IEmailSender email;
        // 加解密
        readonly ITimeLimitedDataProtector protector;
        // 用户管理SDK
        readonly UserManager<AppUser> userManager;
        #endregion

        public UserController(
            ApplicationDbContext _db,
            RedisService _redis,
            IStringLocalizer<UserController> _localizer,
            ISmsSender _sms,
            IEmailSender _email,
            IDataProtectionProvider _provider,
            UserManager<AppUser> _userManager)
        {
            // 多语言
            l = _localizer;
            db = _db;
            redis = _redis;
            sms = _sms;
            protector = _provider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();
            email = _email;
            userManager = _userManager;
        }

        /// <summary>
        /// Get User List
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users, Policy = UserPermissions.Read)]
        [SwaggerOperation("User/Get")]
        public async Task<PagingResult<AppUserModel>> Get(PagingRequest<AppUserQuery> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<AppUserModel>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,

                    error_msg = ModelErrors()
                };
            }

            var query = db.Users.AsQueryable();

                query = query.Where(x => x.Tenants.Any(t => t.AppTenantId == TenantId));

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.Email))
            {
                query = query.Where(x => x.Email.Equals(value.q.Email));
            }

            if (!string.IsNullOrWhiteSpace(value.q.Name))
            {
                query = query.Where(x => x.NickName.Equals(value.q.Name));
            }

            if (!string.IsNullOrWhiteSpace(value.q.PhoneNumber))
            {
                query = query.Where(x => x.PhoneNumber.Equals(value.q.PhoneNumber));
            }

            if (value.q.Role > 0)
            {
                query = query.Where(x => x.Roles.Any(role => role.RoleId == (int)value.q.Role));
            }
            #endregion

            #region total
            var result = new PagingResult<AppUserModel>()
            {
                skip = value.skip,
                take = value.take,
                total = await query.CountAsync()
            }; 
            #endregion

            if (result.total > 0)
            {
                #region orderby
                if (!string.IsNullOrWhiteSpace(value.orderby))
                {
                    if (value.asc)
                    {
                        query = query.OrderBy(value.orderby);
                    }
                    else
                    {
                        query = query.OrderByDescending(value.orderby);
                    }
                }
                #endregion

                #region pagingWithData
                var data = await query.Skip(value.skip).Take(value.take)
                            .Include(x => x.Logins)
                            .Include(x => x.Claims)
                            .Include(x => x.Roles)
                            .Include(x => x.Files)
                            .ToListAsync(); 
                #endregion

                if (data.Count > 0)
                {
                    var models = data.ToModels();

                    var roles = await db.Roles.ToDictionaryAsync(k => k.Id, v => v.NormalizedName);

                    models.ForEach(x => x.Roles.ForEach(r => r.Name = roles[r.Id]));

                    result.data = models;
                }
            }

            return result;
        }

        /// <summary>
        /// Get User Detail By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Read)]
        [SwaggerOperation("User/Detail")]
        public async Task<ApiResult<AppUser>> Get(int id)
        {
            var query = db.Users.AsQueryable();

                query = query.Where(x => x.Tenants.Any(t => t.AppTenantId == TenantId));

            var entity = await query
                .Include(x => x.Logins)
                .Include(x => x.Claims)
                .Include(x => x.Roles)
                .Include(x => x.Files)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return new ApiResult<AppUser>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<AppUser>(entity);
        }

        /// <summary>
        /// Insert User
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users, Policy = UserPermissions.Create)]
        [SwaggerOperation("User/Post")]
        public async Task<ApiResult<long>> Post([FromBody]AppUser value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            value.Tenants.Clear();
            value.Tenants.Add(new AspNetUserTenant() { AppTenantId = TenantId });

            db.Add(value);

            await db.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }

        /// <summary>
        /// Update User
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users, Policy = UserPermissions.Update)]
        [SwaggerOperation("User/Put")]
        public async Task<ApiResult<long>> Put([FromBody]AppUser value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, 
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            using (var tran = db.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    db.Update(value);
                    db.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await db.Users.Where(x => x.Id == value.Id)
                                     .Include(x => x.Logins)
                                     .Include(x => x.Claims)
                                     .Include(x => x.Roles)
                                     .Include(x => x.Files)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();
                    #endregion

                    #region Update Entity.Claims
                    if (value.Claims != null && value.Claims.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Claims.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Claims.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE AspNetUserClaims WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE AspNetUserClaims SET [ClaimType]=@Type,[ClaimValue]=@Value WHERE Id = " + x.Id),
                                  new SqlParameter("@Type", x.ClaimType),
                                  new SqlParameter("@Value", x.ClaimValue));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO AspNetUserClaims VALUES (@ClaimType,@ClaimValue,@UserId)"),
                                  new SqlParameter("@ClaimType", x.ClaimType),
                                  new SqlParameter("@ClaimValue", x.ClaimValue),
                                  new SqlParameter("@UserId", source.Id));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Files
                    if (value.Files != null && value.Files.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Files.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Files.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE AspNetUserFile WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Files.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE AspNetUserFile SET [FileType]=@FileType,[Files]=@Files WHERE Id = " + x.Id),
                                  new SqlParameter("@FileType", x.FileType),
                                  new SqlParameter("@Files", x.Files));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Files.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO AspNetUserFile VALUES (@FileType,@Files,@AppUserId)"),
                                  new SqlParameter("@FileType", x.FileType),
                                  new SqlParameter("@Files", x.Files),
                                  new SqlParameter("@AppUserId", source.Id));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Roles
                    if (value.Roles != null && value.Roles.Count > 0)
                    {
                        #region delete
                        var sql = $"DELETE AspNetUserRoles WHERE UserId = {source.Id}";
                        db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                        #endregion

                        #region insert
                            value.Roles.ForEach(x =>
                            {
                                db.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO AspNetUserRoles VALUES (@UserId,@RoleId)"),
                                  new SqlParameter("@UserId", source.Id),
                                  new SqlParameter("@RoleId", x.RoleId));
                            });
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Roles
                    if (value.Roles != null && value.Roles.Count > 0)
                    {
                        #region delete
                        var sql = $"DELETE AspNetUserRoles WHERE UserId = {source.Id}";
                        db.Database.ExecuteSqlCommand(new RawSqlString(sql));
                        #endregion

                        #region insert
                        value.Roles.ForEach(x =>
                        {
                            db.Database.ExecuteSqlCommand(
                              new RawSqlString("INSERT INTO AspNetUserRoles VALUES (@UserId,@RoleId)"),
                              new SqlParameter("@UserId", source.Id),
                              new SqlParameter("@RoleId", x.RoleId));
                        });
                        #endregion
                    }
                    #endregion

                    tran.Commit();
                }

                catch (Exception ex)
                {
                    tran.Rollback();

                    return new ApiResult<long>(l, 
                        BasicControllerEnums.ExpectationFailed,
                        ex.Message);
                }
            }

            return new ApiResult<long>(value.Id);
        }

        /// <summary>
        /// Delete User
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users, Policy = UserPermissions.Delete)]
        [SwaggerOperation("User/Delete")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            var query = db.Users.AsQueryable();

                query = query.Where(x => x.Tenants.Any(t => t.AppTenantId == TenantId));

            var entity = await query.Where(x => x.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            db.Users.Remove(entity);

            await db.SaveChangesAsync();

            return new ApiResult<long>(id);
        }

        /// <summary>
        /// Check exists
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Head")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Read)]
        [SwaggerOperation("User/Head")]
        public async Task<ObjectResult> Head(AppUserDetailQuery value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(0);
            }

            var query = db.Users.AsQueryable();

                query = query.Where(x => x.Tenants.Any(t => t.AppTenantId == TenantId));

            var result = await query.Where(x => x.PhoneNumber.Equals(value.PhoneNumber))
                .Select(x => x.Id).FirstOrDefaultAsync();

            if (result > 0)
            {
                return new OkObjectResult(result);
            }
            else
            {
                return new NotFoundObjectResult(0);
            }
        }

        #region 开放接口业务
        /// <summary>
        /// 用户报名
        /// 邮箱验证码非必填
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("ApplyFor")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Create)]
        [SwaggerOperation("User/ApplyFor")]
        public async Task<ApiResult<string>> ApplyFor([FromBody]ApplyForModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l,BasicControllerEnums.UnprocessableEntity, 
                    ModelErrors());
            }

            #region 校验邮箱是否重复
            //if (await db.Users.AnyAsync(x => x.Email.Equals(value.Email)))
            //{
            //    return new SingleResult<string>(StatusCodes.Status406NotAcceptable, l["邮箱已被注册"]);
            //}
            #endregion
            #region 校验邮箱验证码
            //if (!string.IsNullOrWhiteSpace(value.EmailVerifyCode))
            //{
            //    try
            //    {
            //        protector.Unprotect(value.EmailVerifyCode);
            //    }
            //    catch
            //    {
            //        return new SingleResult<string>(StatusCodes.Status406NotAcceptable, l["无效的邮箱验证码"]);
            //    }
            //}
            #endregion

            #region 校验手机号是否重复
            if (await db.Users.AnyAsync(x => x.PhoneNumber.Equals(value.PhoneNumber)))
            {
                return new ApiResult<string>(l, UserControllerEnums.ApplyFor.PhoneNumberExists);
            }
            #endregion
            #region 校验手机验证码
            var PhoneNumberVerifyCodeKey = RedisKeys.VerifyCode_Phone + value.PhoneNumber + ":" + value.PhoneNumberVerifyCode;

            if (await redis.KeyExists(PhoneNumberVerifyCodeKey) == false)
            {
                return new ApiResult<string>(l, UserControllerEnums.ApplyFor.PhoneNumberVerifyCodeError);
            }

            await redis.Remove(PhoneNumberVerifyCodeKey);
            #endregion

            #region 创建用户
            var user = new AppUser
            {
                UserName = value.PhoneNumber + "@xcx.com",
                Email = value.PhoneNumber + "@xcx.com",
                PhoneNumber = value.PhoneNumber,
                NickName = value.NickName,
                Gender = value.Gender,
                Address = value.Address,
                Birthday = value.Birthday,
                PhoneNumberConfirmed = true,
                Stature = value.Stature,
                Weight = value.Weight,
                Description = value.Description,
                CreateDate = DateTime.UtcNow,
                LastUpdateTime = DateTime.UtcNow
            };

            // set default tenantId
            user.Tenants.Add(new AspNetUserTenant() { AppTenantId = 1 });

            var result = await userManager.CreateAsync(user, "123456aA!");

            if (result.Succeeded)
            {
                #region 确认邮箱验证通过
                // 如果填写了邮件验证码，并且验证通过（不通过不会走到这里）
                //if (!string.IsNullOrWhiteSpace(value.EmailVerifyCode))
                //{
                //    user.EmailConfirmed = true;
                //}
                #endregion

                #region 设置角色
                var RoleIDs = db.Roles.Where(x => x.Name.Equals(Roles.Users) || x.Name.Equals(Roles.Star)).Select(x => x.Id).ToList();

                var UserRoles = RoleIDs.Select(x => new AppUserRole()
                {
                    RoleId = x,
                    UserId = user.Id
                });

                db.UserRoles.AddRange(UserRoles);
                #endregion

                #region 图片
                if (value.ImageUrl != null && value.ImageUrl.Count > 0)
                {
                    db.UserFiles.Add(new AspNetUserFile()
                    {
                        Files = JsonConvert.SerializeObject(value.ImageUrl),
                        FileType = FileTypes.Image,
                        AppUserId = user.Id
                    });
                }
                #endregion

                #region 视频
                if (!string.IsNullOrWhiteSpace(value.Video))
                {
                    db.UserFiles.Add(new AspNetUserFile()
                    {
                        Files = value.Video,
                        FileType = FileTypes.Video,
                        AppUserId = user.Id
                    });
                }
                #endregion

                #region 文档
                if (!string.IsNullOrWhiteSpace(value.Doc))
                {
                    db.UserFiles.Add(new AspNetUserFile()
                    {
                        Files = value.Doc,
                        FileType = FileTypes.Doc,
                        AppUserId = user.Id
                    });
                }
                #endregion

                await db.SaveChangesAsync();

                #region 发送报名成功的短信通知
                var smsVars = JsonConvert.SerializeObject(new { nickname = value.NickName });
                await sms.SendSmsWithRetryAsync(smsVars, value.PhoneNumber, "9901", 3); 
                #endregion

                return new ApiResult<string>();
            }

            else
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed,
                    JsonConvert.SerializeObject(result.Errors));
            }
            #endregion
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("VerifyPhoneNumber")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Create)]
        [SwaggerOperation("User/VerifyPhoneNumber")]
        public async Task<ApiResult<string>> VerifyPhoneNumber([FromBody]VerifyPhoneNumberModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l,BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 发送计数、验证是否已经达到上限
            var dailyLimitKey = RedisKeys.Limit_24Hour_Verify_Phone + value.PhoneNumber;

            var _dailyLimit = await redis.Get(dailyLimitKey);

            if (!string.IsNullOrWhiteSpace(_dailyLimit))
            {
                var dailyLimit = int.Parse(_dailyLimit);

                if (dailyLimit > RedisKeys.Limit_24Hour_Verify_MAX_Phone)
                {
                    return new ApiResult<string>(l,UserControllerEnums.VerifyPhoneNumber.CallLimited);
                }
            }
            else
            {
                await redis.Set(dailyLimitKey, "0", TimeSpan.FromHours(24));
            }
            #endregion

            #region 验证发送间隔时间是否过快
            //两次发送间隔必须大于指定秒数
            var _lastTimeKey = RedisKeys.LastTime_SendCode_Phone + value.PhoneNumber;

            var lastTimeString = await redis.Get(_lastTimeKey);

            if (!string.IsNullOrWhiteSpace(lastTimeString))
            {
                var lastTime = long.Parse(lastTimeString);

                var now = DateTime.UtcNow.AddHours(8).Ticks;

                var usedTime = (now - lastTime) / 10000000;

                if (usedTime < RedisKeys.MinimumTime_SendCode_Phone)
                {
                    return new ApiResult<string>(l, UserControllerEnums.VerifyPhoneNumber.TooManyRequests, string.Empty,
                        RedisKeys.MinimumTime_SendCode_Phone - usedTime);
                }
            }
            #endregion

            #region 发送验证码
            var verifyCode = random.Next(1111, 9999).ToString();
            var smsVars = JsonConvert.SerializeObject(new { code = verifyCode });
            await sms.SendSmsWithRetryAsync(smsVars, value.PhoneNumber, "9900", 3);
            #endregion

            var verifyCodeKey = RedisKeys.VerifyCode_Phone + value.PhoneNumber + ":" + verifyCode;

            // 记录验证码，用于提交报名接口校验
            await redis.Set(verifyCodeKey, string.Empty, TimeSpan.FromSeconds(RedisKeys.VerifyCode_Expire_Phone));

            // 记录发送验证码的时间，用于下次发送验证码校验间隔时间
            await redis.Set(_lastTimeKey, DateTime.UtcNow.AddHours(8).Ticks.ToString(), null);

            // 叠加发送次数
            await redis.Increment(dailyLimitKey);

            return new ApiResult<string>();
        }

        /// <summary>
        /// 发送邮件验证码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("VerifyEmailAddress")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Create)]
        [SwaggerOperation("User/VerifyEmailAddress")]
        public async Task<ApiResult<string>> VerifyEmailAddress([FromBody]VerifyEmailAddressModel value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l,BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 发送计数、验证是否已经达到上限
            var dailyLimitKey = RedisKeys.Limit_24Hour_Verify_Email + value.Email;

            var _dailyLimit = await redis.Get(dailyLimitKey);

            if (!string.IsNullOrWhiteSpace(_dailyLimit))
            {
                var dailyLimit = int.Parse(_dailyLimit);

                if (dailyLimit > RedisKeys.Limit_24Hour_Verify_MAX_Email)
                {
                    return new ApiResult<string>(l,UserControllerEnums.VerifyEmailAddress.CallLimited);
                }
            }
            else
            {
                await redis.Set(dailyLimitKey, "0", TimeSpan.FromHours(24));
            }
            #endregion

            #region 验证发送间隔时间是否过快
            //两次发送间隔必须大于指定秒数
            var _lastTimeKey = RedisKeys.LastTime_SendCode_Email + value.Email;

            var lastTimeString = await redis.Get(_lastTimeKey);

            if (!string.IsNullOrWhiteSpace(lastTimeString))
            {
                var lastTime = long.Parse(lastTimeString);

                var now = DateTime.UtcNow.AddHours(8).Ticks;

                var usedTime = (now - lastTime) / 10000000;

                if (usedTime < RedisKeys.MinimumTime_SendCode_Email)
                {
                    return new ApiResult<string>(l, UserControllerEnums.VerifyEmailAddress.TooManyRequests, string.Empty,
                        RedisKeys.MinimumTime_SendCode_Email - usedTime);
                }
            }
            #endregion

            #region 发送验证码
            var verifyCode = random.Next(111111, 999999).ToString();
            // 用加密算法生成具有时效性的密文
            var protectedData = protector.Protect(verifyCode, TimeSpan.FromSeconds(RedisKeys.VerifyCode_Expire_Email));
            var xsmtpapi = JsonConvert.SerializeObject(new
            {
                to = new string[] { value.Email },
                sub = new Dictionary<string, string[]>()
                        {
                            { "%code%", new string[] { protectedData } },
                        }
            });
            await email.SendEmailAsync("邮箱验证", "verify_email", xsmtpapi);
            #endregion

            // 记录发送验证码的时间，用于下次发送验证码校验间隔时间
            await redis.Set(_lastTimeKey, DateTime.UtcNow.AddHours(8).Ticks.ToString(), null);

            // 叠加发送次数
            await redis.Increment(dailyLimitKey);

            return new ApiResult<string>();
        }
        #endregion
    }
}
