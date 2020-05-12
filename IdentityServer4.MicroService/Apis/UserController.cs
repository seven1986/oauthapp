using System;
using System.Linq;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Tenant;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.CacheKeys;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.UserController;
using static IdentityServer4.MicroService.AppConstant;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Apis
{
    // user 根据 tenantId 来获取列表、或详情、增删改

    /// <summary>
    /// 用户
    /// </summary>
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("User")]
    [SwaggerTag("用户")]
    public class UserController : ApiControllerBase
    {
        #region Services
        // 短信
        readonly ISmsSender sms;
        // 邮件
        readonly EmailService email;
        // 用户管理SDK
        readonly UserManager<AppUser> userManager;

        readonly TenantDbContext tenantDbContext;

        readonly ConfigurationDbContext configDbContext;

        readonly IdentityServer4MicroServiceOptions ismsOptions;
        #endregion

        #region 构造函数
        public UserController(
            UserDbContext _db,
            RedisService _redis,
            IStringLocalizer<UserController> _localizer,
            ISmsSender _sms,
            EmailService _email,
            UserManager<AppUser> _userManager,
            TenantDbContext _tenantDbContext,
            ConfigurationDbContext _configDbContext,
            IDataProtectionProvider _provider,
            TenantService _tenantService,
            IdentityServer4MicroServiceOptions _ismsOptions)
        {
            // 多语言
            l = _localizer;
            db = _db;
            redis = _redis;
            sms = _sms;
            email = _email;
            userManager = _userManager;
            tenantDbContext = _tenantDbContext;
            configDbContext = _configDbContext;
            protector = _provider.CreateProtector(GetType().FullName).ToTimeLimitedDataProtector();
            tenantService = _tenantService;
            ismsOptions = _ismsOptions;
        }
        #endregion

        #region 用户
        #region 用户 - 列表
        /// <summary>
        /// 用户 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.get")]
        [SwaggerOperation(OperationId = "UserGet",
            Summary = "用户 - 列表",
            Description = "scope&permission：isms.user.get")]
        public async Task<PagingResult<View_User>> Get([FromQuery]PagingRequest<UserGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<View_User>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,

                    message = ModelErrors()
                };
            }

            if (string.IsNullOrWhiteSpace(value.orderby))
            {
                value.orderby = "UserID";
            }

            var q = new PagingService<View_User>(db, value, "View_User")
            {
                where = (where, sqlParams) =>
                {
                    where.Add(" ( Tenants LIKE '%\"TenantId\":" + TenantId + "%') ");

                    if (!User.IsInRole(DefaultRoles.Administrator) && !string.IsNullOrWhiteSpace(UserLineage))
                    {
                        where.Add("Lineage.IsDescendantOf(hierarchyid::Parse ('" + UserLineage + "')) = 1");
                    }

                    if (!string.IsNullOrWhiteSpace(value.q.email))
                    {
                        where.Add("Email = @Email");
                        sqlParams.Add(new SqlParameter("@Email", value.q.email));
                    }

                    if (!string.IsNullOrWhiteSpace(value.q.name))
                    {
                        where.Add("UserName like @UserName");
                        sqlParams.Add(new SqlParameter("@UserName", "%" + value.q.name + "%"));
                    }

                    if (!string.IsNullOrWhiteSpace(value.q.phoneNumber))
                    {
                        where.Add("PhoneNumber = @PhoneNumber");
                        sqlParams.Add(new SqlParameter("@PhoneNumber", value.q.phoneNumber));
                    }

                    if (!string.IsNullOrWhiteSpace(value.q.roles))
                    {
                        var roleIds = value.q.roles.Split(new string[] { "," },
                            StringSplitOptions.RemoveEmptyEntries).ToList();

                        var rolesExpression = roleIds.Select(r => "Roles Like '%\"Id\":" + r + ",%'");

                        where.Add(" ( " + string.Join(" AND ", rolesExpression) + " ) ");
                    }
                }
            };

            var result = await q.ExcuteAsync(propConverter: (prop, val) =>
             {
                 switch (prop.Name)
                 {
                     case "Roles":
                         return JsonConvert.DeserializeObject<List<View_User_Role>>(val.ToString());

                     case "Claims":
                         return JsonConvert.DeserializeObject<List<View_User_Claim>>(val.ToString());

                     case "Files":
                         return JsonConvert.DeserializeObject<List<View_User_File>>(val.ToString());

                     case "Properties":
                         return JsonConvert.DeserializeObject<List<View_User_Property>>(val.ToString());

                     case "Tenants":
                         return JsonConvert.DeserializeObject<List<View_User_Tenant>>(val.ToString());

                     case "Logins":
                         return JsonConvert.DeserializeObject<List<View_User_Login>>(val.ToString());

                     default:
                         return val;
                 }
             });

            return result;
        }
        #endregion

        #region 用户 - 团队
        /// <summary>
        /// 获取团队列表
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet("Distributors")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.distributors")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.distributors")]
        [SwaggerOperation(OperationId = "UserDistributors",
            Summary = "用户 - 团队",
            Description = "scope&permission：isms.user.distributors")]
        public ApiResult<List<DistributorResponse>> Distributors([FromQuery][Required]string path)
        {
            var cmd = @"SELECT
                        A.ID,
                        A.UserName,
                        A.Avatar, 
                        B.Members,
                        B.Sales,
                        A.ParentUserID,
                        A.Lineage.GetLevel() AS LineageLevel,
                        A.Lineage.ToString() AS Lineage
                        FROM AspNetUsers A
                        JOIN AspNetUserDistributors B ON A.ID = B.UserID
                        WHERE A.Lineage.IsDescendantOf(hierarchyid::Parse ('{0}')) = 1";

            if (!User.IsInRole(DefaultRoles.Administrator) && !string.IsNullOrWhiteSpace(UserLineage))
            {
                cmd += @" AND A.Lineage.IsDescendantOf(hierarchyid::Parse ('" + UserLineage + "')) = 1";
            }

            var result = db.DistributorResponse.FromSqlRaw(cmd, path).ToList();

            return new ApiResult<List<DistributorResponse>>(result);
        }
        #endregion

        #region 用户 - 详情
        /// <summary>
        ///用户 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.detail")]
        [SwaggerOperation(OperationId = "UserDetail",
            Summary = "用户 - 详情",
            Description = "scope&permission：isms.user.detail")]
        public async Task<ApiResult<AppUser>> Get(int id)
        {
            var query = db.Users.AsQueryable();

            query = query.Where(x => x.Tenants.Any(t => t.TenantId == TenantId));

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
        #endregion

        #region 用户 - 创建
        /// <summary>
        /// 用户 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.post")]
        [SwaggerOperation(OperationId = "UserPost",
            Summary = "用户 - 创建",
            Description = "scope&permission：isms.user.post")]
        public async Task<ApiResult<long>> Post([FromBody]AppUser value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            var roleIds = db.Roles.Where(x => x.Name.Equals(DefaultRoles.User) || x.Name.Equals(DefaultRoles.Developer))
                  .Select(x => x.Id).ToList();

            var tenantIds = tenantDb.Tenants.Select(x => x.Id).ToList();
            try
            {
                var result = await AppUserService.CreateUser(
                    TenantId,
                    userManager,
                    db,
                    value,
                    roleIds,
                    $"{AppConstant.MicroServiceName}.all",
                    tenantIds);

                db.Add(value);

                if (result.Succeeded)
                {
                    return new ApiResult<long>(value.Id);
                }

                else
                {
                    return new ApiResult<long>(l, UserControllerEnums.Post_CreateUserFail,
                       JsonConvert.SerializeObject(result.Errors));
                }
            }

            catch (Exception ex)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.ExpectationFailed,
                    ex.Message);
            }
        }
        #endregion

        #region 用户 - 更新
        /// <summary>
        /// 用户 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.put")]
        [SwaggerOperation(OperationId = "UserPut",
            Summary = "用户 - 更新",
            Description = "scope&permission：isms.user.put")]
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
                                //var sql = string.Format("DELETE AspNetUserClaims WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlRaw($"DELETE AspNetUserClaims WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlRaw($"UPDATE AspNetUserClaims SET [ClaimType]={x.ClaimType},[ClaimValue]={x.ClaimValue} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlRaw($"INSERT INTO AspNetUserClaims VALUES ({x.ClaimType},{x.ClaimValue},{source.Id})");
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
                                //var sql = string.Format("DELETE AspNetUserFiles WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlRaw($"DELETE AspNetUserFiles WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Files.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlRaw($"UPDATE AspNetUserFiles SET [FileType]={x.FileType},[Files]={x.Files} WHERE Id ={x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Files.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlRaw(
                                  $"INSERT INTO AspNetUserFiles VALUES ({x.FileType},{x.Files},{source.Id})");
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Roles
                    if (value.Roles != null && value.Roles.Count > 0)
                    {
                        #region delete
                        //var sql = $"DELETE AspNetUserRoles WHERE UserId = {source.Id}";
                        db.Database.ExecuteSqlRaw($"DELETE AspNetUserRoles WHERE UserId = {source.Id}");
                        #endregion

                        #region insert
                        value.Roles.ForEach(x =>
                        {
                            db.Database.ExecuteSqlRaw(
                              $"INSERT INTO AspNetUserRoles VALUES ({source.Id},{x.RoleId})");
                        });
                        #endregion
                    }
                    #endregion

                    #region Update Entity.Properties
                    if (value.Properties != null && value.Properties.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.Properties.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.Properties.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                //var sql = string.Format("DELETE AspNetUserProperties WHERE ID IN ({0})",
                                //            string.Join(",", DeleteEntities));

                                db.Database.ExecuteSqlRaw($"DELETE AspNetUserProperties WHERE ID IN ({string.Join(",", DeleteEntities)})");
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Properties.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlRaw($"UPDATE AspNetUserProperties SET [Key]={x.Key},[Value]={x.Value} WHERE Id = {x.Id}");
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Properties.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                db.Database.ExecuteSqlRaw($"INSERT INTO AspNetUserProperties VALUES ({x.Key},{source.Id},{x.Value})");
                            });
                        }
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
        #endregion

        #region 用户 - 删除
        /// <summary>
        /// 用户 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.delete")]
        [SwaggerOperation(OperationId = "UserDelete",
            Summary = "用户 - 删除",
            Description = "scope&permission：isms.user.delete")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            var query = db.Users.AsQueryable();

            query = query.Where(x => x.Tenants.Any(t => t.TenantId == TenantId));

            var entity = await query.Where(x => x.Id == id && !x.IsDeleted).FirstOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            entity.IsDeleted = true;

            //db.Users.Remove(entity);

            await db.SaveChangesAsync();

            return new ApiResult<long>(id);
        }
        #endregion

        #region 用户 - 是否存在
        /// <summary>
        /// 用户 - 是否存在
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet("Head")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:user.head")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:user.head")]
        [SwaggerOperation(OperationId = "UserHead",
            Summary = "用户 - 是否存在",
            Description = "scope&permission：isms.user.head")]
        public async Task<ObjectResult> Head(UserDetailRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(0);
            }

            var query = db.Users.AsQueryable();

            query = query.Where(x => x.Tenants.Any(t => t.TenantId == TenantId));

            var result = await query.Where(x => x.PhoneNumber.Equals(value.PhoneNumber) && !x.IsDeleted)
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
        #endregion

        #region 用户 - 错误码表
        /// <summary>
        /// 用户 - 错误码表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "UserCodes",
            Summary = "用户 - 错误码表",
            Description = "用户错误码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<UserControllerEnums>();

            return result;
        }
        #endregion
        #endregion

        #region 用户 - 注册
        #region 用户 - 注册 - 提交
        /// <summary>
        /// 用户 - 注册 - 提交
        /// </summary>
        /// <returns></returns>
        [HttpPost("Register")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "UserRegister",
            Summary = "用户 - 注册 - 提交",
            Description = "需验证手机号；邮箱如果填写了，也需要验证")]
        public async Task<ApiResult<string>> Register([FromBody]UserRegisterRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 校验邮箱是否重复
            if (await db.Users.AnyAsync(x => x.Email.Equals(value.Email)))
            {
                return new ApiResult<string>(l, UserControllerEnums.Register_EmailExists);
            }
            #endregion
            #region 校验邮箱验证码
            if (!string.IsNullOrWhiteSpace(value.EmailVerifyCode))
            {
                try
                {
                    var UnprotectStr = Unprotect(value.EmailVerifyCode);
                }
                catch
                {
                    return new ApiResult<string>(l, UserControllerEnums.Register_EmailVerifyCodeError);
                }
            }
            #endregion

            #region 校验手机号是否重复
            if (await db.Users.AnyAsync(x => x.PhoneNumber.Equals(value.PhoneNumber) && x.CountryCode == value.CountryCode))
            {
                return new ApiResult<string>(l, UserControllerEnums.Register_PhoneNumberExists);
            }
            #endregion
            #region 校验手机验证码
            var PhoneNumberVerifyCodeKey = UserControllerKeys.VerifyCode_Phone + value.PhoneNumber + ":" + value.PhoneNumberVerifyCode;

            if (await redis.KeyExistsAsync(PhoneNumberVerifyCodeKey) == false)
            {
                return new ApiResult<string>(l, UserControllerEnums.PhoneNumberVerifyCodeError);
            }

            await redis.RemoveAsync(PhoneNumberVerifyCodeKey);
            #endregion

            #region 创建用户
            var user = new AppUser
            {
                UserName = value.Email,
                Email = value.Email,
                PhoneNumber = value.PhoneNumber,
                NickName = value.NickName,
                Gender = value.Gender,
                Address = value.Address,
                Birthday = value.Birthday,
                PhoneNumberConfirmed = true,
                Stature = value.Stature,
                Weight = value.Weight,
                Description = value.Description ?? "",
                CreateDate = DateTime.UtcNow,
                LastUpdateTime = DateTime.UtcNow,
                EmailConfirmed = true,
                ParentUserID = value.RefereeID,
                CountryCode = value.CountryCode,
                PasswordHash = value.Password
            };

            #region 确认邮箱验证通过
            //如果填写了邮件验证码，并且验证通过（不通过不会走到这里）
            if (!string.IsNullOrWhiteSpace(value.EmailVerifyCode))
            {
                user.EmailConfirmed = true;
            }
            #endregion

            #region 图片
            if (value.ImageUrl != null && value.ImageUrl.Count > 0)
            {
                user.Files.Add(new AspNetUserFile()
                {
                    Files = JsonConvert.SerializeObject(value.ImageUrl),
                    FileType = FileTypes.Image,
                });
            }
            #endregion

            #region 视频
            if (!string.IsNullOrWhiteSpace(value.Video))
            {
                user.Files.Add(new AspNetUserFile()
                {
                    Files = value.Video,
                    FileType = FileTypes.Video,
                });
            }
            #endregion

            #region 文档
            if (!string.IsNullOrWhiteSpace(value.Doc))
            {
                user.Files.Add(new AspNetUserFile()
                {
                    Files = value.Doc,
                    FileType = FileTypes.Doc,
                });
            }
            #endregion

            var roleIds = db.Roles.Where(x => x.Name.Equals(DefaultRoles.User))
                    .Select(x => x.Id).ToList();

            var tenantIds = tenantDbContext.Tenants.Select(x => x.Id).ToList();

            var result = await AppUserService.CreateUser(1L,
                userManager,
                db,
                user,
                roleIds,
                $"{AppConstant.MicroServiceName}.all",
                tenantIds);

            if (result.Succeeded)
            {
                return new ApiResult<string>(user.Id.ToString());
            }

            else
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed,
                    JsonConvert.SerializeObject(result.Errors));
            }
            #endregion
        }
        #endregion

        #region 用户 - 注册 - 发送手机验证码
        /// <summary>
        /// 用户 - 注册 - 发送手机验证码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("SmsCode")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "UserSmsCode",
            Summary = "用户 - 注册 - 发送手机验证码",
            Description = "")]
        public async Task<ApiResult<string>> SmsCode([FromBody]UserVerifyPhoneRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 发送计数、验证是否已经达到上限
            var dailyLimitKey = UserControllerKeys.Limit_24Hour_Verify_Phone + value.PhoneNumber;

            var _dailyLimit = await redis.GetAsync(dailyLimitKey);

            if (!string.IsNullOrWhiteSpace(_dailyLimit))
            {
                var dailyLimit = int.Parse(_dailyLimit);

                if (dailyLimit > UserControllerKeys.Limit_24Hour_Verify_MAX_Phone)
                {
                    return new ApiResult<string>(l, UserControllerEnums.VerifyPhone_CallLimited);
                }
            }
            else
            {
                await redis.SetAsync(dailyLimitKey, "0", TimeSpan.FromHours(24));
            }
            #endregion

            #region 验证发送间隔时间是否过快
            //两次发送间隔必须大于指定秒数
            var _lastTimeKey = UserControllerKeys.LastTime_SendCode_Phone + value.PhoneNumber;

            var lastTimeString = await redis.GetAsync(_lastTimeKey);

            if (!string.IsNullOrWhiteSpace(lastTimeString))
            {
                var lastTime = long.Parse(lastTimeString);

                var now = DateTime.UtcNow.AddHours(8).Ticks;

                var usedTime = (now - lastTime) / 10000000;

                if (usedTime < UserControllerKeys.MinimumTime_SendCode_Phone)
                {
                    return new ApiResult<string>(l, UserControllerEnums.VerifyPhone_TooManyRequests, string.Empty,
                        UserControllerKeys.MinimumTime_SendCode_Phone - usedTime);
                }
            }
            #endregion

            #region 发送验证码
            var verifyCode = random.Next(1111, 9999).ToString();
            var smsVars = JsonConvert.SerializeObject(new { code = verifyCode });
            await sms.SendSmsWithRetryAsync(smsVars, value.PhoneNumber, "9900", 3);
            #endregion

            var verifyCodeKey = UserControllerKeys.VerifyCode_Phone + value.PhoneNumber + ":" + verifyCode;

            // 记录验证码，用于提交报名接口校验
            await redis.SetAsync(verifyCodeKey, string.Empty, TimeSpan.FromSeconds(UserControllerKeys.VerifyCode_Expire_Phone));

            // 记录发送验证码的时间，用于下次发送验证码校验间隔时间
            await redis.SetAsync(_lastTimeKey, DateTime.UtcNow.AddHours(8).Ticks.ToString(), null);

            // 叠加发送次数
            await redis.IncrementAsync(dailyLimitKey);

            return new ApiResult<string>();
        }
        #endregion

        #region 用户 - 注册 - 发送邮件验证码
        /// <summary>
        /// 用户 - 注册 - 发送邮件验证码
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("EmailCode")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "UserEmailCode",
            Summary = "用户 - 注册 - 发送邮件验证码",
            Description = "")]
        public async Task<ApiResult<string>> EmailCode([FromBody]UserVerifyEmailRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 发送计数、验证是否已经达到上限
            var dailyLimitKey = UserControllerKeys.Limit_24Hour_Verify_Email + value.Email;

            var _dailyLimit = await redis.GetAsync(dailyLimitKey);

            if (!string.IsNullOrWhiteSpace(_dailyLimit))
            {
                var dailyLimit = int.Parse(_dailyLimit);

                if (dailyLimit > UserControllerKeys.Limit_24Hour_Verify_MAX_Email)
                {
                    return new ApiResult<string>(l, UserControllerEnums.VerifyEmail_CallLimited);
                }
            }
            else
            {
                await redis.SetAsync(dailyLimitKey, "0", TimeSpan.FromHours(24));
            }
            #endregion

            #region 验证发送间隔时间是否过快
            //两次发送间隔必须大于指定秒数
            var _lastTimeKey = UserControllerKeys.LastTime_SendCode_Email + value.Email;

            var lastTimeString = await redis.GetAsync(_lastTimeKey);

            if (!string.IsNullOrWhiteSpace(lastTimeString))
            {
                var lastTime = long.Parse(lastTimeString);

                var now = DateTime.UtcNow.AddHours(8).Ticks;

                var usedTime = (now - lastTime) / 10000000;

                if (usedTime < UserControllerKeys.MinimumTime_SendCode_Email)
                {
                    return new ApiResult<string>(l, UserControllerEnums.VerifyEmail_TooManyRequests, string.Empty,
                        UserControllerKeys.MinimumTime_SendCode_Email - usedTime);
                }
            }
            #endregion

            #region 发送验证码
            var verifyCode = random.Next(111111, 999999).ToString();

            verifyCode = Protect(verifyCode,
                TimeSpan.FromSeconds(UserControllerKeys.VerifyCode_Expire_Email));

            await email.SendEmailAsync("verify_email", "邮箱验证",
                //SendCloudMailTemplates.verify_email,
               new string[] { value.Email },
                new Dictionary<string, string[]>() {
                    { "%code%", new string[] { verifyCode } }
                });
            #endregion

            // 记录发送验证码的时间，用于下次发送验证码校验间隔时间
            await redis.SetAsync(_lastTimeKey, DateTime.UtcNow.AddHours(8).Ticks.ToString(), null);

            // 叠加发送次数
            await redis.IncrementAsync(dailyLimitKey);

            return new ApiResult<string>();
        }
        #endregion 

        #region 用户 - 忘记密码 - 手机验证码
        /// <summary>
        /// 用户 - 忘记密码 - 手机验证码
        /// </summary>
        /// <returns></returns>
        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "UserResetPassword",
            Summary = "用户 - 忘记密码 - 手机验证码",
            Description = "")]
        public async Task<ApiResult<bool>> ResetPassword([FromBody]ResetPasswordRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            #region 校验手机验证码
            var PhoneNumberVerifyCodeKey = UserControllerKeys.VerifyCode_Phone + value.PhoneNumber + ":" + value.PhoneNumberVerifyCode;

            if (await redis.KeyExistsAsync(PhoneNumberVerifyCodeKey) == false)
            {
                return new ApiResult<bool>(l, UserControllerEnums.PhoneNumberVerifyCodeError);
            }

            await redis.RemoveAsync(PhoneNumberVerifyCodeKey);
            #endregion

            var User = await db.Users.FirstOrDefaultAsync(x =>
            x.PhoneNumber == value.PhoneNumber && x.CountryCode == value.CountryCode);

            if (User==null)
            {
                return new ApiResult<bool>(l, UserControllerEnums.Register_PhoneNumberExists);
            }

            User.PasswordHash = userManager.PasswordHasher.HashPassword(User, value.Password);

            db.SaveChanges();

            return new ApiResult<bool>(true);
        }
        #endregion
        #endregion
    }
}