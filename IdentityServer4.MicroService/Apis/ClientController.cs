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
    [SwaggerTag("客户端")]
    public class ClientController : ApiControllerBase
    {
        #region Services
        // database for identityserver
        readonly ConfigurationDbContext idsDB;
        // database for user
        readonly UserDbContext userDB;
        // IdentityServer Tools
        readonly IdentityServerTools _tools;
        #endregion

        #region 构造函数
        public ClientController(
            ConfigurationDbContext _idsDB,
            UserDbContext _userDB,
            IStringLocalizer<ClientController> localizer,
            IdentityServerTools tools)
        {
            userDB = _userDB;
            idsDB = _idsDB;
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
            Description = "scope&permission：isms.client.get")]
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

            var query = idsDB.Clients.AsQueryable();

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
            Description = "scope&permission：isms.client.detail")]
        public async Task<ApiResult<Client>> Get(int id)
        {
            if (!userDB.UserClients
              .Any(x => x.UserId == UserId && x.ClientId == id))
            {
                return new ApiResult<Client>(l, BasicControllerEnums.NotFound);
            }

            var query = idsDB.Clients.AsQueryable();

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
            Description = "scope&permission：isms.client.post")]
        public async Task<ApiResult<long>> Post([FromBody]Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            idsDB.Add(value);

            try
            {
                await idsDB.SaveChangesAsync();
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
            Description = "scope&permission：isms.client.put")]
        public ApiResult<bool> Put([FromBody]Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<bool>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

           if(!userDB.UserClients
               .Any(x => x.UserId == UserId && x.ClientId == value.Id))
            {
                return new ApiResult<bool>(l, BasicControllerEnums.NotFound)
                {
                    data = false
                };
            }

            var Entity = idsDB.Clients
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.Claims)
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
            Entity.AccessTokenLifetime = value.AccessTokenLifetime;
            Entity.AuthorizationCodeLifetime = value.AuthorizationCodeLifetime;
            Entity.ConsentLifetime = value.ConsentLifetime;
            Entity.AbsoluteRefreshTokenLifetime = value.AbsoluteRefreshTokenLifetime;
            Entity.SlidingRefreshTokenLifetime = value.SlidingRefreshTokenLifetime;
            Entity.RefreshTokenUsage = value.RefreshTokenUsage;
            Entity.UpdateAccessTokenClaimsOnRefresh = value.UpdateAccessTokenClaimsOnRefresh;
            Entity.RefreshTokenExpiration = value.RefreshTokenExpiration;
            Entity.AccessTokenType = value.AccessTokenType;
            Entity.EnableLocalLogin = value.EnableLocalLogin;
            Entity.IncludeJwtId = value.IncludeJwtId;
            Entity.AlwaysSendClientClaims = value.AlwaysSendClientClaims;
            Entity.ClientClaimsPrefix = value.ClientClaimsPrefix;
            Entity.PairWiseSubjectSalt = value.PairWiseSubjectSalt;
            Entity.Created = value.Created;
            Entity.Updated = value.Updated;
            Entity.LastAccessed = value.LastAccessed;
            Entity.UserSsoLifetime = value.UserSsoLifetime;
            Entity.UserCodeType = value.UserCodeType;
            Entity.AllowedIdentityTokenSigningAlgorithms = value.AllowedIdentityTokenSigningAlgorithms;
            Entity.IdentityTokenLifetime = value.IdentityTokenLifetime;
            Entity.AllowOfflineAccess = value.AllowOfflineAccess;
            Entity.Enabled = value.Enabled;
            Entity.ClientId = value.ClientId;
            Entity.ProtocolType = value.ProtocolType;
            Entity.RequireClientSecret = value.RequireClientSecret;
            Entity.ClientName = value.ClientName;
            Entity.Description = value.Description;
            Entity.ClientUri = value.ClientUri;
            Entity.LogoUri = value.LogoUri;
            Entity.RequireConsent = value.RequireConsent;
            Entity.DeviceCodeLifetime = value.DeviceCodeLifetime;
            Entity.AllowRememberConsent = value.AllowRememberConsent;
            Entity.RequirePkce = value.RequirePkce;
            Entity.AllowPlainTextPkce = value.AllowPlainTextPkce;
            Entity.RequireRequestObject = value.RequireRequestObject;
            Entity.AllowAccessTokensViaBrowser = value.AllowAccessTokensViaBrowser;
            Entity.FrontChannelLogoutUri = value.FrontChannelLogoutUri;
            Entity.FrontChannelLogoutSessionRequired = value.FrontChannelLogoutSessionRequired;
            Entity.BackChannelLogoutUri = value.BackChannelLogoutUri;
            Entity.BackChannelLogoutSessionRequired = value.BackChannelLogoutSessionRequired;
            Entity.AlwaysIncludeUserClaimsInIdToken = value.AlwaysIncludeUserClaimsInIdToken;
            Entity.NonEditable = value.NonEditable;
            #endregion

            #region IdentityProviderRestrictions
            if (Entity.IdentityProviderRestrictions != null && Entity.IdentityProviderRestrictions.Count > 0)
            {
                Entity.IdentityProviderRestrictions.Clear();
            }
            if (value.IdentityProviderRestrictions != null && value.IdentityProviderRestrictions.Count > 0)
            {
                value.IdentityProviderRestrictions.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Provider))
                    {
                        Entity.IdentityProviderRestrictions.Add(new ClientIdPRestriction()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            Provider = x.Provider
                        });
                    }
                });
            }
            #endregion

            #region Claims
            if (Entity.Claims != null && Entity.Claims.Count > 0)
            {
                Entity.Claims.Clear();
            }
            if (value.Claims != null && value.Claims.Count > 0)
            {
                value.Claims.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Type))
                    {
                        Entity.Claims.Add(new ClientClaim()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            Type = x.Type,
                            Value = x.Value
                        });
                    }
                });
            }
            #endregion

            #region AllowedCorsOrigins
            if (Entity.AllowedCorsOrigins != null && Entity.AllowedCorsOrigins.Count > 0)
            {
                Entity.AllowedCorsOrigins.Clear();
            }
            if (value.AllowedCorsOrigins != null && value.AllowedCorsOrigins.Count > 0)
            {
                value.AllowedCorsOrigins.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Origin))
                    {
                        Entity.AllowedCorsOrigins.Add(new ClientCorsOrigin()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            Origin = x.Origin
                        });
                    }
                });
            }
            #endregion

            #region Properties
            if (Entity.Properties != null && Entity.Properties.Count > 0)
            {
                Entity.Properties.Clear();
            }
            if (value.Properties != null && value.Properties.Count > 0)
            {
                value.Properties.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Key))
                    {
                        Entity.Properties.Add(new ClientProperty()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            Key = x.Key,
                            Value = x.Value
                        });
                    }
                });
            }
            #endregion

            #region AllowedScopes
            if (Entity.AllowedScopes != null && Entity.AllowedScopes.Count > 0)
            {
                Entity.AllowedScopes.Clear();
            }
            if (value.AllowedScopes != null && value.AllowedScopes.Count > 0)
            {
                value.AllowedScopes.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Scope))
                    {
                        Entity.AllowedScopes.Add(new ClientScope()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            Scope = x.Scope
                        });
                    }
                });
            }
            #endregion

            #region ClientSecrets
            if (Entity.ClientSecrets != null && Entity.ClientSecrets.Count > 0)
            {
                Entity.ClientSecrets.Clear();
            }
            if (value.ClientSecrets != null && value.ClientSecrets.Count > 0)
            {
                value.ClientSecrets.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.Type) && !string.IsNullOrWhiteSpace(x.Value))
                    {
                        Entity.ClientSecrets.Add(new ClientSecret()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            Type = x.Type,
                            Value = x.Value,
                            Created = x.Created,
                            Description = x.Description,
                            Expiration = x.Expiration
                        });
                    }
                });
            }
            #endregion

            #region AllowedGrantTypes
            if (Entity.AllowedGrantTypes != null && Entity.AllowedGrantTypes.Count > 0)
            {
                Entity.AllowedGrantTypes.Clear();
            }
            if (value.AllowedGrantTypes != null && value.AllowedGrantTypes.Count > 0)
            {
                value.AllowedGrantTypes.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.GrantType))
                    {
                        Entity.AllowedGrantTypes.Add(new ClientGrantType()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            GrantType = x.GrantType
                        });
                    }
                });
            }
            #endregion

            #region RedirectUris
            if (Entity.RedirectUris != null && Entity.RedirectUris.Count > 0)
            {
                Entity.RedirectUris.Clear();
            }
            if (value.RedirectUris != null && value.RedirectUris.Count > 0)
            {
                value.RedirectUris.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.RedirectUri))
                    {
                        Entity.RedirectUris.Add(new ClientRedirectUri()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            RedirectUri = x.RedirectUri
                        });
                    }
                });
            }
            #endregion

            #region PostLogoutRedirectUris
            if (Entity.PostLogoutRedirectUris != null && Entity.PostLogoutRedirectUris.Count > 0)
            {
                Entity.PostLogoutRedirectUris.Clear();
            }
            if (value.PostLogoutRedirectUris != null && value.PostLogoutRedirectUris.Count > 0)
            {
                value.PostLogoutRedirectUris.ForEach(x =>
                {
                    if (!string.IsNullOrWhiteSpace(x.PostLogoutRedirectUri))
                    {
                        Entity.PostLogoutRedirectUris.Add(new ClientPostLogoutRedirectUri()
                        {
                            Client = Entity,
                            ClientId = value.Id,
                            PostLogoutRedirectUri = x.PostLogoutRedirectUri
                        });
                    }
                });
            }
            #endregion

            

            try
            {
                idsDB.SaveChanges();
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
            Description = "scope&permission：isms.client.delete")]
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
                var entity = idsDB.Clients.Where(m => m.Id == id)
                .Include(x => x.Claims)
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.Properties)
                .Include(x => x.AllowedScopes)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.IdentityProviderRestrictions)
                .Include(x => x.RedirectUris).FirstOrDefault();

                idsDB.Clients.Remove(entity);

                idsDB.SaveChanges();
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
            Description = "scope&permission：isms.client.issuetoken")]
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
            Description = "scope&permission：isms.client.postsecretkey")]
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