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
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Models.Apis.Common;
using IdentityServer4.MicroService.Models.Apis.ClientController;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    // Client 根据 userId 来获取列表、或详情、增删改

    /// <summary>
    /// 客户端
    /// </summary>
    [Produces("application/json")]
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
            if (!await exists(id))
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
        public async Task<ApiResult<long>> Put([FromBody]Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (!await exists(value.Id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }
            

            idsDB.Attach(value).State = EntityState.Modified;

            try
            {
                idsDB.SaveChanges();
            }

            catch (Exception ex)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.ExpectationFailed,
                    ex.Message);
            }

            return new ApiResult<long>(value.Id);

            //using (var tran = idsDB.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            //{
            //    try
            //    {
            //        #region Update Entity
            //        // 需要先更新value，否则更新如claims等属性会有并发问题
            //        idsDB.Update(value);
            //        idsDB.SaveChanges();
            //        #endregion

            //        #region Find Entity.Source
            //        var source = await idsDB.Clients.Where(x => x.Id == value.Id)
            //                        .Include(x => x.Claims)
            //                        .Include(x => x.AllowedGrantTypes)
            //                        .Include(x => x.AllowedScopes)
            //                        .Include(x => x.ClientSecrets)
            //                        .Include(x => x.AllowedCorsOrigins)
            //                        .Include(x => x.RedirectUris)
            //                        .Include(x => x.PostLogoutRedirectUris)
            //                        .Include(x => x.IdentityProviderRestrictions)
            //                        .Include(x => x.Properties)
            //                        .AsNoTracking()
            //                        .FirstOrDefaultAsync();
            //        #endregion

            //        #region Update Entity.Claims
            //        if (value.Claims != null && value.Claims.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.Claims.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.Claims.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientClaims WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientClaims WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientClaims SET [Type]={x.Type},[Value]={x.Value} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientClaims VALUES ({source.Id},{x.Type},{ x.Value})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.AllowedGrantTypes
            //        if (value.AllowedGrantTypes != null && value.AllowedGrantTypes.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.AllowedGrantTypes.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.AllowedGrantTypes.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientGrantTypes WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientGrantTypes WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.AllowedGrantTypes.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientGrantTypes SET [GrantType]= {x.GrantType} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.AllowedGrantTypes.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientGrantTypes VALUES ({source.Id},{x.GrantType})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.AllowedScopes
            //        if (value.AllowedScopes != null && value.AllowedScopes.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.AllowedScopes.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.AllowedScopes.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientScopes WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientScopes WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.AllowedScopes.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientScopes SET [Scope]= {x.Scope} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.AllowedScopes.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientScopes VALUES ({source.Id},{x.Scope})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.ClientSecrets
            //        if (value.ClientSecrets != null && value.ClientSecrets.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.ClientSecrets.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.ClientSecrets.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientSecrets WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientSecrets WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.ClientSecrets.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    //var sql = new RawSqlString("UPDATE ClientSecrets SET [Description]=@Description,[Expiration]=@Expiration,[Type]=@Type,[Value]=@Value WHERE Id = " + x.Id);

            //                    //var _params = new SqlParameter[]
            //                    //{
            //                    //    new SqlParameter("@Description", DBNull.Value){ IsNullable=true },
            //                    //    new SqlParameter("@Expiration",DBNull.Value){ IsNullable=true },
            //                    //    new SqlParameter("@Type",DBNull.Value){ IsNullable=true },
            //                    //    new SqlParameter("@Value", DBNull.Value){ IsNullable=true },
            //                    //};

            //                    //if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
            //                    //if (x.Expiration.HasValue) { _params[1].Value = x.Expiration; }
            //                    //if (!string.IsNullOrWhiteSpace(x.Type)) { _params[2].Value = x.Type; }
            //                    //if (!string.IsNullOrWhiteSpace(x.Value)) { _params[3].Value = x.Value; }

            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientSecrets SET [Description]={x.Description},[Expiration]={x.Expiration},[Type]={x.Type},[Value]={x.Value} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.ClientSecrets.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    //var sql = new RawSqlString("INSERT INTO ClientSecrets VALUES (@ClientId,@Description,@Expiration,@Type,@Value)");

            //                    //var _params = new SqlParameter[]
            //                    //{
            //                    //    new SqlParameter("@ClientId", source.Id),
            //                    //    new SqlParameter("@Description", DBNull.Value){ IsNullable=true },
            //                    //    new SqlParameter("@Expiration", DBNull.Value){ IsNullable=true },
            //                    //    new SqlParameter("@Type", DBNull.Value){ IsNullable=true },
            //                    //    new SqlParameter("@Value", DBNull.Value){ IsNullable=true },
            //                    //};

            //                    //if (!string.IsNullOrWhiteSpace(x.Description)) { _params[1].Value = x.Description; }
            //                    //if (x.Expiration.HasValue) { _params[2].Value = x.Expiration; }
            //                    //if (!string.IsNullOrWhiteSpace(x.Type)) { _params[3].Value = x.Type; }
            //                    //if (!string.IsNullOrWhiteSpace(x.Value)) { _params[4].Value = x.Value; }

            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientSecrets VALUES ({source.Id},{x.Description},{x.Expiration},{x.Type},{x.Value})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.AllowedCorsOrigins
            //        if (value.AllowedCorsOrigins != null && value.AllowedCorsOrigins.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.AllowedCorsOrigins.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.AllowedCorsOrigins.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientCorsOrigins WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientCorsOrigins WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.AllowedCorsOrigins.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientCorsOrigins SET [Origin]={x.Origin} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.AllowedCorsOrigins.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientCorsOrigins VALUES ({source.Id},{x.Origin})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.RedirectUris
            //        if (value.RedirectUris != null && value.RedirectUris.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.RedirectUris.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.RedirectUris.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientRedirectUris WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientRedirectUris WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.RedirectUris.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientRedirectUris SET [RedirectUri]= {x.RedirectUri} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.RedirectUris.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientRedirectUris VALUES ({source.Id},{x.RedirectUri})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.PostLogoutRedirectUris
            //        if (value.PostLogoutRedirectUris != null && value.PostLogoutRedirectUris.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.PostLogoutRedirectUris.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.PostLogoutRedirectUris.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientPostLogoutRedirectUris WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientPostLogoutRedirectUris WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.PostLogoutRedirectUris.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientPostLogoutRedirectUris SET [PostLogoutRedirectUri]= {x.PostLogoutRedirectUri} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.PostLogoutRedirectUris.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientPostLogoutRedirectUris VALUES ({source.Id},{x.PostLogoutRedirectUri})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.IdentityProviderRestrictions
            //        if (value.IdentityProviderRestrictions != null && value.IdentityProviderRestrictions.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.IdentityProviderRestrictions.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.IdentityProviderRestrictions.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientIdPRestrictions WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientIdPRestrictions WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.IdentityProviderRestrictions.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientIdPRestrictions SET [Provider]={x.Provider} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.IdentityProviderRestrictions.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientIdPRestrictions VALUES ({source.Id},{x.Provider})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        #region Update Entity.Properties
            //        if (value.Properties != null && value.Properties.Count > 0)
            //        {
            //            #region delete
            //            var EntityIDs = value.Properties.Select(x => x.Id).ToList();
            //            if (EntityIDs.Count > 0)
            //            {
            //                var DeleteEntities = source.Properties.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

            //                if (DeleteEntities.Count() > 0)
            //                {
            //                    //var sql = string.Format("DELETE ClientProperties WHERE ID IN ({0})",
            //                    //            string.Join(",", DeleteEntities));

            //                    idsDB.Database.ExecuteSqlRaw($"DELETE ClientProperties WHERE ID IN ({string.Join(",", DeleteEntities)})");
            //                }
            //            }
            //            #endregion

            //            #region update
            //            var UpdateEntities = value.Properties.Where(x => x.Id > 0).ToList();
            //            if (UpdateEntities.Count > 0)
            //            {
            //                UpdateEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"UPDATE ClientProperties SET [Key]={x.Key},[Value]={x.Value} WHERE Id = {x.Id}");
            //                });
            //            }
            //            #endregion

            //            #region insert
            //            var NewEntities = value.Properties.Where(x => x.Id == 0).ToList();
            //            if (NewEntities.Count > 0)
            //            {
            //                NewEntities.ForEach(x =>
            //                {
            //                    idsDB.Database.ExecuteSqlRaw($"INSERT INTO ClientProperties VALUES ({source.Id},{x.Key},{x.Value})");
            //                });
            //            }
            //            #endregion
            //        }
            //        #endregion

            //        tran.Commit();
            //    }

            //    catch (Exception ex)
            //    {
            //        tran.Rollback();

            //        return new ApiResult<long>(l,
            //            BasicControllerEnums.ExpectationFailed,
            //            ex.Message);
            //    }
            //}
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
        public async Task<ApiResult<long>> Delete(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }
            try
            {
                var entity = await idsDB.Clients.SingleOrDefaultAsync(m => m.Id == id);

                idsDB.Clients.Remove(entity);

                await idsDB.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ApiResult<long>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source);
            }

            var sql = "DELETE AspNetUserClients WHERE ClientId=@ClientId AND UserId=@UserId";

            var _params = new SqlParameter[]
            {
                new SqlParameter("@ClientId",id),
                new SqlParameter("@UserId",UserId),
            };
            try
            {
                await userDB.Database.ExecuteSqlRawAsync(sql, _params);
            }
            catch (Exception ex)
            {
                return new ApiResult<long>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source);
            }
            return new ApiResult<long>(id);
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

            var excludeClaimTypes = new List<String>() { "nbf", "exp", "iss" };

            var claims = User.Claims.Where(x => !excludeClaimTypes.Contains(x.Type)).ToList();

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

        #region 辅助方法
        async Task<bool> exists(long id)
        {
            var query = idsDB.Clients.AsQueryable();

            var clientIDs = await userDB.UserClients
                .Where(x => x.UserId == UserId)
                .Select(x => x.ClientId).ToListAsync();

            query = query.Where(x => clientIDs.Contains(x.Id));

            return query.Any(x => x.Id == id);
        }
        #endregion
    }
}