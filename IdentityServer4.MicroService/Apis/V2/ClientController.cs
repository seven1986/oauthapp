using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Models.CommonModels;
using static IdentityServer4.MicroService.AppConstant;
using IdentityServer4.MicroService.Models.ClientModels;

namespace IdentityServer4.MicroService.Apis.V2
{
    /// <summary>
    /// JUST FOR TEST
    /// </summary>
    //[ApiVersion("2.0")] // 正式版
    //[ApiVersion("2.0-Beta")]    // 公开测试版
    //[ApiVersion("2.0-RC")] // 候选版本
    //[ApiVersion("2017-11-06.1-RC")] // 带日期的候选版本
    [ApiVersion("2.0-Alpha")] // 内部测试版
    [Route("Client")]
    [Route("v{version:apiVersion}/Client")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class ClientController : BasicController
    {
        #region Database
        readonly ConfigurationDbContext idsDB;
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
            var ClientIDs = userDB.UserClients.Where(x => x.UserId == UserId).Select(x => x.ClientId).ToList();

            var data = await idsDB.Clients.Where(x => ClientIDs.Contains(x.Id))
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

            var total = await idsDB.Clients.CountAsync();

            return new PagingResult<Client>(data, total, value.skip, value.take);
        }
    }
}
