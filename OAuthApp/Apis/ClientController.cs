using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using OAuthApp.Data;
using OAuthApp.Enums;
using OAuthApp.Models.Apis.Common;
using OAuthApp.Models.Apis.ClientController;
using static OAuthApp.AppConstant;
using IdentityServer4;
using System.Security.Claims;

namespace OAuthApp.Apis
{
    /// <summary>
    /// 客户端
    /// </summary>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Client")]
    [SwaggerTag("#### 客户端管理")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class ClientController : ApiControllerBase
    {
        #region Services
        // database for identityserver
        readonly ConfigurationDbContext configDb;
        // database for user
        readonly UserDbContext userDB;
        // IdentityServer Tools
        readonly IdentityServerTools _tools;
        #endregion

        #region 构造函数
        public ClientController(
            ConfigurationDbContext _configDb,
            UserDbContext _userDB,
            IStringLocalizer<ClientController> localizer,
            IdentityServerTools tools)
        {
            userDB = _userDB;
            configDb = _configDb;
            l = localizer;
            _tools = tools;
        }
        #endregion

        #region 客户端 - 列表
        /// <summary>
        /// 客户端 - 列表
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.get")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.get")]
        [SwaggerOperation(
            OperationId = "ClientGet",
            Summary = "客户端 - 列表",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.get | oauthapp.client.get |")]
        public async Task<PagingResult<Client>> Get([FromQuery]PagingRequest<ClientGetRequest> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<Client>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var query = configDb.Clients.AsQueryable();

            var clientIDs = await userDB.UserClients.Where(x => x.UserId == UserId)
               .Select(x => x.ClientId).ToListAsync();

            //if (clientIDs.Count > 0)
            //{
                query = query.Where(x => clientIDs.Contains(x.Id));
            //}

            #region filter
            if (!string.IsNullOrWhiteSpace(value.q.ClientID))
            {
                query = query.Where(x => x.ClientId.Equals(value.q.ClientID));
            }

            if (!string.IsNullOrWhiteSpace(value.q.ClientName))
            {
                query = query.Where(x => x.ClientName.Equals(value.q.ClientName));
            }
            #endregion

            #region total
            var result = new PagingResult<Client>()
            {
                skip = value.skip.Value,
                take = value.take.Value,
                total = await query.CountAsync()
            };
            #endregion

            if (result.total > 0)
            {
                #region orderby
                if (!string.IsNullOrWhiteSpace(value.orderby))
                {
                    if (value.asc.Value)
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
                var data = await query
               .Skip(value.skip.Value).Take(value.take.Value)
               .Include(x => x.Claims)
               .Include(x => x.AllowedGrantTypes)
               .Include(x => x.AllowedScopes)
               .Include(x => x.ClientSecrets)
               .Include(x => x.AllowedCorsOrigins)
               .Include(x => x.RedirectUris)
               .Include(x => x.PostLogoutRedirectUris)
               .Include(x => x.IdentityProviderRestrictions)
               .Include(x => x.Properties)
               .ToListAsync();
                #endregion

                result.data = data;
            }

            return result;
        }
        #endregion

        #region 客户端 - 详情
        /// <summary>
        /// 客户端 - 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.detail")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.detail")]
        [SwaggerOperation(OperationId = "ClientDetail",
            Summary = "客户端 - 详情",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.detail | oauthapp.client.detail |")]
        public async Task<ApiResult<Client>> Get(int id)
        {
            if (!userDB.UserClients
              .Any(x => x.UserId == UserId && x.ClientId == id))
            {
                return new ApiResult<Client>(l, BasicControllerEnums.NotFound);
            }

            var query = configDb.Clients.AsQueryable();

            var clientIDs = await userDB.UserClients.Where(x => x.UserId == UserId)
             .Select(x => x.ClientId).ToListAsync();

            var entity = await query
                .Where(x => x.Id == id && clientIDs.Contains(x.Id))
                .Include(x => x.Claims)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.Properties)
                .SingleOrDefaultAsync();

            if (entity == null)
            {
                return new ApiResult<Client>(l, BasicControllerEnums.NotFound);
            }

            return new ApiResult<Client>(entity);
        }
        #endregion

        #region 客户端 - 创建
        /// <summary>
        /// 客户端 - 创建
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.post")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.post")]
        [SwaggerOperation(OperationId = "ClientPost",
            Summary = "客户端 - 创建",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.post | oauthapp.client.post |")]
        public async Task<ApiResult<long>> Post([FromBody]Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            configDb.Add(value);

            try
            {
                await configDb.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ApiResult<long>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.StackTrace);
            }

            userDB.UserClients.Add(new AspNetUserClient()
            {
                ClientId = value.Id,
                UserId = UserId
            });

            await userDB.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }
        #endregion

        #region 客户端 - 更新
        /// <summary>
        /// 客户端 - 更新
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.put")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.put")]
        [SwaggerOperation(OperationId = "ClientPut",
            Summary = "客户端 - 更新",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.put | oauthapp.client.put |")]
        public ApiResult<bool> Put([FromBody] Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!userDB.UserClients
                .Any(x => x.UserId == UserId && x.ClientId == value.Id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            var Entity = configDb.Clients
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.Claims)
                .Include(x => x.AllowedScopes)
                .Include(x => x.Properties)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .FirstOrDefault(x => x.Id == value.Id);

            if (Entity == null)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            #region Properties
            if (Entity.AccessTokenLifetime != value.AccessTokenLifetime)
            {
                Entity.AccessTokenLifetime = value.AccessTokenLifetime;
            }
            if (Entity.AuthorizationCodeLifetime != value.AuthorizationCodeLifetime)
            {
                Entity.AuthorizationCodeLifetime = value.AuthorizationCodeLifetime;
            }
            if (value.ConsentLifetime.HasValue)
            {
                Entity.ConsentLifetime = value.ConsentLifetime;
            }
            if (Entity.AbsoluteRefreshTokenLifetime != value.AbsoluteRefreshTokenLifetime)
            {
                Entity.AbsoluteRefreshTokenLifetime = value.AbsoluteRefreshTokenLifetime;
            }
            if (Entity.SlidingRefreshTokenLifetime != value.SlidingRefreshTokenLifetime)
            {
                Entity.SlidingRefreshTokenLifetime = value.SlidingRefreshTokenLifetime;
            }
            if (Entity.RefreshTokenUsage != value.RefreshTokenUsage)
            {
                Entity.RefreshTokenUsage = value.RefreshTokenUsage;
            }
            if (Entity.UpdateAccessTokenClaimsOnRefresh != value.UpdateAccessTokenClaimsOnRefresh)
            {
                Entity.UpdateAccessTokenClaimsOnRefresh = value.UpdateAccessTokenClaimsOnRefresh;
            }
            if (Entity.RefreshTokenExpiration != value.RefreshTokenExpiration)
            {
                Entity.RefreshTokenExpiration = value.RefreshTokenExpiration;
            }
            if (Entity.AccessTokenType != value.AccessTokenType)
            {
                Entity.AccessTokenType = value.AccessTokenType;
            }
            if (Entity.EnableLocalLogin != value.EnableLocalLogin)
            {
                Entity.EnableLocalLogin = value.EnableLocalLogin;
            }
            if (Entity.IncludeJwtId != value.IncludeJwtId)
            {
                Entity.IncludeJwtId = value.IncludeJwtId;
            }
            if (Entity.AlwaysSendClientClaims != value.AlwaysSendClientClaims)
            {
                Entity.AlwaysSendClientClaims = value.AlwaysSendClientClaims;
            }
            if (!string.IsNullOrWhiteSpace(value.ClientClaimsPrefix) &&
                Entity.ClientClaimsPrefix!=value.ClientClaimsPrefix)
            {
                Entity.ClientClaimsPrefix = value.ClientClaimsPrefix;
            }
            if (!string.IsNullOrWhiteSpace(value.PairWiseSubjectSalt) &&
                Entity.PairWiseSubjectSalt != value.PairWiseSubjectSalt)
            {
                Entity.PairWiseSubjectSalt = value.PairWiseSubjectSalt;
            }
            if (value.Updated.HasValue)
            {
                Entity.Updated = value.Updated;
            }
            if (value.LastAccessed.HasValue)
            {
                Entity.LastAccessed = value.LastAccessed;
            }
            if (value.UserSsoLifetime.HasValue)
            {
                Entity.UserSsoLifetime = value.UserSsoLifetime;
            }
            if (Entity.AllowOfflineAccess != value.AllowOfflineAccess) { Entity.AllowOfflineAccess = value.AllowOfflineAccess; }
            if (Entity.IdentityTokenLifetime != value.IdentityTokenLifetime) { Entity.IdentityTokenLifetime = value.IdentityTokenLifetime; }
            if (Entity.RequireClientSecret != value.RequireClientSecret) { Entity.RequireClientSecret = value.RequireClientSecret; }
            if (Entity.Created != value.Created) { Entity.Created = value.Created; }
            if (Entity.RequireConsent != value.RequireConsent) { Entity.RequireConsent = value.RequireConsent; }
            if (Entity.DeviceCodeLifetime != value.DeviceCodeLifetime) { Entity.DeviceCodeLifetime = value.DeviceCodeLifetime; }
            if (Entity.AllowRememberConsent != value.AllowRememberConsent) { Entity.AllowRememberConsent = value.AllowRememberConsent; }
            if (Entity.RequirePkce != value.RequirePkce) { Entity.RequirePkce = value.RequirePkce; }
            if (Entity.AllowPlainTextPkce != value.AllowPlainTextPkce) { Entity.AllowPlainTextPkce = value.AllowPlainTextPkce; }
            if (Entity.RequireRequestObject != value.RequireRequestObject) { Entity.RequireRequestObject = value.RequireRequestObject; }
            if (Entity.AllowAccessTokensViaBrowser != value.AllowAccessTokensViaBrowser) { Entity.AllowAccessTokensViaBrowser = value.AllowAccessTokensViaBrowser; }
            if (Entity.FrontChannelLogoutSessionRequired != value.FrontChannelLogoutSessionRequired) { Entity.FrontChannelLogoutSessionRequired = value.FrontChannelLogoutSessionRequired; }
            if (Entity.BackChannelLogoutSessionRequired != value.BackChannelLogoutSessionRequired) { Entity.BackChannelLogoutSessionRequired = value.BackChannelLogoutSessionRequired; }
            if (Entity.AlwaysIncludeUserClaimsInIdToken != value.AlwaysIncludeUserClaimsInIdToken) { Entity.AlwaysIncludeUserClaimsInIdToken = value.AlwaysIncludeUserClaimsInIdToken; }
            if (Entity.NonEditable != value.NonEditable) { Entity.NonEditable = value.NonEditable; }
            if (Entity.Enabled != value.Enabled) { Entity.Enabled = value.Enabled; }
            if (!string.IsNullOrWhiteSpace(value.UserCodeType) &&
                Entity.UserCodeType != value.UserCodeType) 
            { Entity.UserCodeType = value.UserCodeType; }
            if (!string.IsNullOrWhiteSpace(value.AllowedIdentityTokenSigningAlgorithms) &&
                Entity.AllowedIdentityTokenSigningAlgorithms != value.AllowedIdentityTokenSigningAlgorithms)
            { Entity.AllowedIdentityTokenSigningAlgorithms = value.AllowedIdentityTokenSigningAlgorithms; }
            if (!string.IsNullOrWhiteSpace(value.ClientId) &&
                Entity.ClientId != value.ClientId) 
            { Entity.ClientId = value.ClientId; }
            if (!string.IsNullOrWhiteSpace(value.ProtocolType) &&
                Entity.ProtocolType != value.ProtocolType) 
            { Entity.ProtocolType = value.ProtocolType; }
            if (!string.IsNullOrWhiteSpace(value.ClientName) &&
                Entity.ClientName != value.ClientName) 
            { Entity.ClientName = value.ClientName; }
            if (!string.IsNullOrWhiteSpace(value.Description) &&
                Entity.Description!=value.Description) 
            { Entity.Description = value.Description; }
            if (!string.IsNullOrWhiteSpace(value.ClientUri) &&
                Entity.ClientUri != value.ClientUri) 
            { Entity.ClientUri = value.ClientUri; }
            if (!string.IsNullOrWhiteSpace(value.LogoUri) &&
                Entity.LogoUri != value.LogoUri) 
            { Entity.LogoUri = value.LogoUri; }
            if (!string.IsNullOrWhiteSpace(value.FrontChannelLogoutUri) &&
                Entity.FrontChannelLogoutUri != value.FrontChannelLogoutUri) 
            { Entity.FrontChannelLogoutUri = value.FrontChannelLogoutUri; }
            if (!string.IsNullOrWhiteSpace(value.BackChannelLogoutUri) &&
                Entity.BackChannelLogoutUri != value.BackChannelLogoutUri) 
            { Entity.BackChannelLogoutUri = value.BackChannelLogoutUri; }
            #endregion

            #region IdentityProviderRestrictions
            if (Entity.IdentityProviderRestrictions != null && Entity.IdentityProviderRestrictions.Count > 0)
            {
                Entity.IdentityProviderRestrictions.Clear();
            }
            if (value.IdentityProviderRestrictions != null && value.IdentityProviderRestrictions.Count > 0)
            {
                Entity.IdentityProviderRestrictions = value.IdentityProviderRestrictions
                    .Where(x => !string.IsNullOrWhiteSpace(x.Provider))
                    .Select(x => new ClientIdPRestriction()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        Provider = x.Provider
                    }).ToList();
            }
            #endregion

            #region Claims
            if (Entity.Claims != null && Entity.Claims.Count > 0)
            {
                Entity.Claims.Clear();
            }
            if (value.Claims != null && value.Claims.Count > 0)
            {
                Entity.Claims = value.Claims
                    .Where(x => !string.IsNullOrWhiteSpace(x.Type))
                    .Select(x => new ClientClaim()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        Type = x.Type,
                        Value = x.Value
                    }).ToList();
            }
            #endregion

            #region AllowedCorsOrigins
            if (Entity.AllowedCorsOrigins != null && Entity.AllowedCorsOrigins.Count > 0)
            {
                Entity.AllowedCorsOrigins.Clear();
            }
            if (value.AllowedCorsOrigins != null && value.AllowedCorsOrigins.Count > 0)
            {
                Entity.AllowedCorsOrigins = value.AllowedCorsOrigins
                    .Where(x => !string.IsNullOrWhiteSpace(x.Origin))
                    .Select(x => new ClientCorsOrigin()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        Origin = x.Origin
                    }).ToList();
            }
            #endregion

            #region Properties
            if (Entity.Properties != null && Entity.Properties.Count > 0)
            {
                Entity.Properties.Clear();
            }
            if (value.Properties != null && value.Properties.Count > 0)
            {
                Entity.Properties = value.Properties
                    .Where(x => !string.IsNullOrWhiteSpace(x.Key))
                    .Select(x => new ClientProperty()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        Key = x.Key,
                        Value = x.Value
                    }).ToList();
            }
            #endregion

            #region AllowedScopes
            if (Entity.AllowedScopes != null && Entity.AllowedScopes.Count > 0)
            {
                Entity.AllowedScopes.Clear();
            }
            if (value.AllowedScopes != null && value.AllowedScopes.Count > 0)
            {
                Entity.AllowedScopes = value.AllowedScopes
                    .Where(x => !string.IsNullOrWhiteSpace(x.Scope))
                    .Select(x => new ClientScope()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        Scope = x.Scope
                    }).ToList();
            }
            #endregion

            #region ClientSecrets
            if (Entity.ClientSecrets != null && Entity.ClientSecrets.Count > 0)
            {
                Entity.ClientSecrets.Clear();
            }
            if (value.ClientSecrets != null && value.ClientSecrets.Count > 0)
            {
                Entity.ClientSecrets = value.ClientSecrets
                    .Where(x => !string.IsNullOrWhiteSpace(x.Type) && !string.IsNullOrWhiteSpace(x.Value))
                    .Select(x => new ClientSecret()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        Type = x.Type,
                        Value = x.Value,
                        Created = x.Created,
                        Description = x.Description,
                        Expiration = x.Expiration.GetValueOrDefault()
                    }).ToList();
            }
            #endregion

            #region AllowedGrantTypes
            if (Entity.AllowedGrantTypes != null && Entity.AllowedGrantTypes.Count > 0)
            {
                Entity.AllowedGrantTypes.Clear();
            }
            if (value.AllowedGrantTypes != null && value.AllowedGrantTypes.Count > 0)
            {
                Entity.AllowedGrantTypes = value.AllowedGrantTypes
                    .Where(x => !string.IsNullOrWhiteSpace(x.GrantType))
                    .Select(x => new ClientGrantType()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        GrantType = x.GrantType
                    }).ToList();
            }
            #endregion

            #region RedirectUris
            if (Entity.RedirectUris != null && Entity.RedirectUris.Count > 0)
            {
                Entity.RedirectUris.Clear();
            }
            if (value.RedirectUris != null && value.RedirectUris.Count > 0)
            {
                Entity.RedirectUris = value.RedirectUris
                    .Where(x => !string.IsNullOrWhiteSpace(x.RedirectUri))
                    .Select(x => new ClientRedirectUri()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        RedirectUri = x.RedirectUri
                    }).ToList();
            }
            #endregion

            #region PostLogoutRedirectUris
            if (Entity.PostLogoutRedirectUris != null && Entity.PostLogoutRedirectUris.Count > 0)
            {
                Entity.PostLogoutRedirectUris.Clear();
            }
            if (value.PostLogoutRedirectUris != null && value.PostLogoutRedirectUris.Count > 0)
            {
                Entity.PostLogoutRedirectUris = value.PostLogoutRedirectUris
                    .Where(x => !string.IsNullOrWhiteSpace(x.PostLogoutRedirectUri))
                    .Select(x => new ClientPostLogoutRedirectUri()
                    {
                        Client = Entity,
                        ClientId = value.Id,
                        PostLogoutRedirectUri = x.PostLogoutRedirectUri
                    }).ToList();
            }
            #endregion

            try
            {
                configDb.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.ExpectationFailed,
                    ex.Message);
            }

            return new ApiResult<bool>(true);
        }
        #endregion

        #region 客户端 - 删除
        /// <summary>
        /// 客户端 - 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.delete")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.delete")]
        [SwaggerOperation(OperationId = "ClientDelete",
            Summary = "客户端 - 删除",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.delete | oauthapp.client.delete |")]
        public ApiResult<bool> Delete(int id)
        {
            if (!userDB.UserClients
              .Any(x => x.UserId == UserId && x.ClientId == id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }
            try
            {
                var entity = configDb.Clients.Where(m => m.Id == id)
                .Include(x => x.Claims)
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.Properties)
                .Include(x => x.AllowedScopes)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.RedirectUris).FirstOrDefault();

                configDb.Clients.Remove(entity);

                configDb.SaveChanges();
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source)
                {
                    data = false
                };
            }

            var sql = "DELETE AspNetUserClients WHERE ClientId=@ClientId AND UserId=@UserId";

            var _params = new SqlParameter[]
            {
                new SqlParameter("@ClientId",id),
                new SqlParameter("@UserId",UserId),
            };
            try
            {
                userDB.Database.ExecuteSqlRaw(sql, _params);
            }
            catch (Exception ex)
            {
                return new ApiResult<bool>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source)
                {
                    data = false
                };
            }
            return new ApiResult<bool>(true);
        }
        #endregion

        #region 客户端 - 创建令牌
        /// <summary>
        /// 客户端 - 创建令牌
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("IssueToken")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.issuetoken")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.issuetoken")]
        [SwaggerOperation(OperationId = "ClientIssueToken",
            Summary = "客户端 - 创建令牌",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.issuetoken | oauthapp.client.issuetoken |")]
        public async Task<ApiResult<string>> IssueToken([FromBody]ClientIssueTokenRequest value)
        {
            if (value.lifetime < 1) { value.lifetime = 3600; }

            var excludeClaimTypes = new List<string>() { "nbf", "exp", "iss" };

            var claims = ((ClaimsIdentity)User.Identity).Claims.Where(x => !excludeClaimTypes.Contains(x.Type)).ToList();

            var token = await _tools.IssueJwtAsync(value.lifetime, claims);

            return new ApiResult<string>(token);
        }
        #endregion

        #region 客户端 - 生成密钥
        /// <summary>
        /// 客户端 - 生成密钥
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost("{id}/Secretkey")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:client.postsecretkey")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "permission:client.postsecretkey")]
        [SwaggerOperation(OperationId = "ClientPostSecretkey",
            Summary = "客户端 - 生成密钥",
            Description = "#### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.client.postsecretkey | oauthapp.client.postsecretkey |")]
        public ApiResult<string> PostSecretkey(int id,[FromBody]ClientPostSecretkeyRequest value)
        {
            var result = string.Empty;

            switch (value.keyType)
            {
                case SecretKeyType.Sha512:
                    result = IdentityServer4.Models.HashExtensions.Sha512(value.plaintext);
                    break;

                case SecretKeyType.Sha256:
                default:
                    result = IdentityServer4.Models.HashExtensions.Sha256(value.plaintext);
                    break;
            }

            return new ApiResult<string>(result);
        }
        #endregion

        #region 客户端 - 错误码表
        /// <summary>
        /// 客户端 - 错误码表
        /// </summary>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "ClientCodes",
            Summary = "客户端 - 错误码表",
            Description = "客户端代码对照表")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<ClientControllerEnums>();

            return result;
        }
        #endregion
    }
}