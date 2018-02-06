using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Codes;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.ClientModels;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    // Client 根据 userId 来获取列表、或详情、增删改

    [Route("Client")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class ClientController : BasicController
    {
        #region Services
        // database for identityserver
        readonly ConfigurationDbContext idsDB;
        // database for user
        readonly ApplicationDbContext userDB;
        #endregion

        public ClientController(
            ConfigurationDbContext _idsDB,
            ApplicationDbContext _userDB,
            IStringLocalizer<ClientController> localizer)
        {
            userDB = _userDB;
            idsDB = _idsDB;
            l = localizer;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("Client/Get")]
        public async Task<PagingResult<Client>> Get(PagingRequest<ClientQuery> value)
        {
            if (!ModelState.IsValid)
            {
                return new PagingResult<Client>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    error_msg = ModelErrors()
                };
            }

            var query = idsDB.Clients.AsQueryable();

            var clientIDs = await userDB.UserClients.Where(x => x.UserId == UserId)
               .Select(x => x.ClientId).ToListAsync();

            if (clientIDs.Count > 0)
            {
                query = query.Where(x => clientIDs.Contains(x.Id));
            }

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
                var data = await query
               .Skip(value.skip).Take(value.take)
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

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Read)]
        [SwaggerOperation("Client/Detail")]
        public async Task<ApiResult<Client>> Get(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<Client>(l, BasicControllerEnums.NotFound);
            }

            var query = idsDB.Clients.AsQueryable();

            var entity = await query
                .Where(x => x.Id == id)
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
        
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Create)]
        [SwaggerOperation("Client/Post")]
        public async Task<ApiResult<long>> Post([FromBody]Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l, BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            idsDB.Add(value);

            await idsDB.SaveChangesAsync();

            userDB.UserClients.Add(new AspNetUserClient()
            {
                ClientId = value.Id,
                UserId = UserId
            });

            await userDB.SaveChangesAsync();

            return new ApiResult<long>(value.Id);
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Update)]
        [SwaggerOperation("Client/Put")]
        public async Task<ApiResult<long>> Put([FromBody]Client value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<long>(l,
                    BasicControllerEnums.UnprocessableEntity,
                    ModelErrors());
            }

            if (! await exists(value.Id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            using (var tran = idsDB.Database.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    #region Update Entity
                    // 需要先更新value，否则更新如claims等属性会有并发问题
                    idsDB.Update(value);
                    idsDB.SaveChanges();
                    #endregion

                    #region Find Entity.Source
                    var source = await idsDB.Clients.Where(x => x.Id == value.Id)
                                    .Include(x => x.Claims)
                                    .Include(x => x.AllowedGrantTypes)
                                    .Include(x => x.AllowedScopes)
                                    .Include(x => x.ClientSecrets)
                                    .Include(x => x.AllowedCorsOrigins)
                                    .Include(x => x.RedirectUris)
                                    .Include(x => x.PostLogoutRedirectUris)
                                    .Include(x => x.IdentityProviderRestrictions)
                                    .Include(x => x.Properties)
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
                                var sql = string.Format("DELETE ClientClaims WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Claims.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientClaims SET [Type]=@Type,[Value]=@Value WHERE Id = " + x.Id),
                                  new SqlParameter("@Type", x.Type),
                                  new SqlParameter("@Value", x.Value));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Claims.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientClaims VALUES (@ClientId,@Type,@Value)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@Type", x.Type),
                                  new SqlParameter("@Value", x.Value));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.AllowedGrantTypes
                    if (value.AllowedGrantTypes != null && value.AllowedGrantTypes.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.AllowedGrantTypes.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.AllowedGrantTypes.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientGrantTypes WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.AllowedGrantTypes.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientGrantTypes SET [GrantType]=@GrantType WHERE Id = " + x.Id),
                                  new SqlParameter("@GrantType", x.GrantType));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.AllowedGrantTypes.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientGrantTypes VALUES (@ClientId,@GrantType)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@GrantType", x.GrantType));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.AllowedScopes
                    if (value.AllowedScopes != null && value.AllowedScopes.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.AllowedScopes.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.AllowedScopes.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientScopes WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.AllowedScopes.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientScopes SET [Scope]=@Scope WHERE Id = " + x.Id),
                                  new SqlParameter("@Scope", x.Scope));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.AllowedScopes.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientScopes VALUES (@ClientId,@Scope)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@Scope", x.Scope));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.ClientSecrets
                    if (value.ClientSecrets != null && value.ClientSecrets.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.ClientSecrets.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.ClientSecrets.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientSecrets WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.ClientSecrets.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                var sql = new RawSqlString("UPDATE ClientSecrets SET [Description]=@Description,[Expiration]=@Expiration,[Type]=@Type,[Value]=@Value WHERE Id = " + x.Id);

                                var _params = new SqlParameter[]
                                {
                                    new SqlParameter("@Description", DBNull.Value){ IsNullable=true },
                                    new SqlParameter("@Expiration",DBNull.Value){ IsNullable=true },
                                    new SqlParameter("@Type",DBNull.Value){ IsNullable=true },
                                    new SqlParameter("@Value", DBNull.Value){ IsNullable=true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.Description)) { _params[0].Value = x.Description; }
                                if (x.Expiration.HasValue) { _params[1].Value = x.Expiration; }
                                if (!string.IsNullOrWhiteSpace(x.Type)) { _params[2].Value = x.Type; }
                                if (!string.IsNullOrWhiteSpace(x.Value)) { _params[3].Value = x.Value; }

                                idsDB.Database.ExecuteSqlCommand(sql, _params);
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.ClientSecrets.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                var sql = new RawSqlString("INSERT INTO ClientSecrets VALUES (@ClientId,@Description,@Expiration,@Type,@Value)");

                                var _params = new SqlParameter[]
                                {
                                    new SqlParameter("@ClientId", source.Id),
                                    new SqlParameter("@Description", DBNull.Value){ IsNullable=true },
                                    new SqlParameter("@Expiration", DBNull.Value){ IsNullable=true },
                                    new SqlParameter("@Type", DBNull.Value){ IsNullable=true },
                                    new SqlParameter("@Value", DBNull.Value){ IsNullable=true },
                                };

                                if (!string.IsNullOrWhiteSpace(x.Description)) { _params[1].Value = x.Description; }
                                if (x.Expiration.HasValue) { _params[2].Value = x.Expiration; }
                                if (!string.IsNullOrWhiteSpace(x.Type)) { _params[3].Value = x.Type; }
                                if (!string.IsNullOrWhiteSpace(x.Value)) { _params[4].Value = x.Value; }

                                idsDB.Database.ExecuteSqlCommand(sql, _params);
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.AllowedCorsOrigins
                    if (value.AllowedCorsOrigins != null && value.AllowedCorsOrigins.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.AllowedCorsOrigins.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.AllowedCorsOrigins.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientCorsOrigins WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.AllowedCorsOrigins.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientCorsOrigins SET [Origin]=@Origin WHERE Id = " + x.Id),
                                  new SqlParameter("@Origin", x.Origin));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.AllowedCorsOrigins.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientCorsOrigins VALUES (@ClientId,@Origin)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@Origin", x.Origin));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.RedirectUris
                    if (value.RedirectUris != null && value.RedirectUris.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.RedirectUris.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.RedirectUris.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientRedirectUris WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.RedirectUris.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientRedirectUris SET [RedirectUri]=@RedirectUri WHERE Id = " + x.Id),
                                  new SqlParameter("@RedirectUri", x.RedirectUri));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.RedirectUris.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientRedirectUris VALUES (@ClientId,@RedirectUri)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@RedirectUri", x.RedirectUri));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.PostLogoutRedirectUris
                    if (value.PostLogoutRedirectUris != null && value.PostLogoutRedirectUris.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.PostLogoutRedirectUris.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.PostLogoutRedirectUris.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientPostLogoutRedirectUris WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.PostLogoutRedirectUris.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientPostLogoutRedirectUris SET [PostLogoutRedirectUri]=@PostLogoutRedirectUri WHERE Id = " + x.Id),
                                  new SqlParameter("@PostLogoutRedirectUri", x.PostLogoutRedirectUri));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.PostLogoutRedirectUris.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientPostLogoutRedirectUris VALUES (@ClientId,@PostLogoutRedirectUri)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@PostLogoutRedirectUri", x.PostLogoutRedirectUri));
                            });
                        }
                        #endregion
                    }
                    #endregion

                    #region Update Entity.IdentityProviderRestrictions
                    if (value.IdentityProviderRestrictions != null && value.IdentityProviderRestrictions.Count > 0)
                    {
                        #region delete
                        var EntityIDs = value.IdentityProviderRestrictions.Select(x => x.Id).ToList();
                        if (EntityIDs.Count > 0)
                        {
                            var DeleteEntities = source.IdentityProviderRestrictions.Where(x => !EntityIDs.Contains(x.Id)).Select(x => x.Id).ToArray();

                            if (DeleteEntities.Count() > 0)
                            {
                                var sql = string.Format("DELETE ClientIdPRestrictions WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.IdentityProviderRestrictions.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientIdPRestrictions SET [Provider]=@Provider WHERE Id = " + x.Id),
                                  new SqlParameter("@Provider", x.Provider));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.IdentityProviderRestrictions.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientIdPRestrictions VALUES (@ClientId,@Provider)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@Provider", x.Provider));
                            });
                        }
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
                                var sql = string.Format("DELETE ClientProperties WHERE ID IN ({0})",
                                            string.Join(",", DeleteEntities));

                                idsDB.Database.ExecuteSqlCommand(new RawSqlString(sql));
                            }
                        }
                        #endregion

                        #region update
                        var UpdateEntities = value.Properties.Where(x => x.Id > 0).ToList();
                        if (UpdateEntities.Count > 0)
                        {
                            UpdateEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("UPDATE ClientProperties SET [Key]=@Key,[Value]=@Value WHERE Id = " + x.Id),
                                  new SqlParameter("@Key", x.Key),
                                  new SqlParameter("@Value", x.Value));
                            });
                        }
                        #endregion

                        #region insert
                        var NewEntities = value.Properties.Where(x => x.Id == 0).ToList();
                        if (NewEntities.Count > 0)
                        {
                            NewEntities.ForEach(x =>
                            {
                                idsDB.Database.ExecuteSqlCommand(
                                  new RawSqlString("INSERT INTO ClientProperties VALUES (@ClientId,@Key,@Value)"),
                                  new SqlParameter("@ClientId", source.Id),
                                  new SqlParameter("@Key", x.Key),
                                  new SqlParameter("@Value", x.Value));
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

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = UserPermissions.Delete)]
        [SwaggerOperation("Client/Delete")]
        public async Task<ApiResult<long>> Delete(int id)
        {
            if (!await exists(id))
            {
                return new ApiResult<long>(l, BasicControllerEnums.NotFound);
            }

            var entity = await idsDB.Clients.SingleOrDefaultAsync(m => m.Id == id);

            idsDB.Clients.Remove(entity);

            await idsDB.SaveChangesAsync();

            var sql = new RawSqlString("DELETE AspNetUserClient WHERE ClientId=@ClientId AND UserId=@UserId");

            var _params = new SqlParameter[]
            {
                new SqlParameter("@ClientId",id),
                new SqlParameter("@UserId",UserId),
            };

            await userDB.Database.ExecuteSqlCommandAsync(sql, _params);

            return new ApiResult<long>(id);
        }

        async Task<bool> exists(long id)
        {
            var query = idsDB.Clients.AsQueryable();

            var clientIDs = await userDB.UserClients
                .Where(x => x.UserId == UserId)
                .Select(x => x.ClientId).ToListAsync();

            if (clientIDs.Count > 0)
            {
                query = query.Where(x => clientIDs.Contains(x.Id));
            }

            return query.Any(x => x.Id == id);
        }
    }
}