using System.Collections.Generic;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.SwaggerGen;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    [Route("CodeGen")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class CodeGenController : BasicController
    {
        #region Services
        readonly SwaggerCodeGenService swagerCodeGen;
        #endregion

        public CodeGenController(
            IStringLocalizer<CodeGenController> localizer,
            SwaggerCodeGenService _swagerCodeGen)
        {
            l = localizer;
            swagerCodeGen = _swagerCodeGen;
        }

        /// <summary>
        /// Swagger CodeGen Clients
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        [HttpGet("Clients")]
        [AllowAnonymous]
        [SwaggerOperation("CodeGen/Clients")]
        public ApiResult<List<SwaggerCodeGenItem>> Clients(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ClientItemsCache : swagerCodeGen.ClientItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }

        /// <summary>
        /// Swagger CodeGen Servers
        /// </summary>
        /// <param name="fromCache"></param>
        /// <returns></returns>
        [HttpGet("Servers")]
        [AllowAnonymous]
        [SwaggerOperation("CodeGen/Servers")]
        public ApiResult<List<SwaggerCodeGenItem>> Servers(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ServerItemsCache : swagerCodeGen.ServerItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }
    }
}
