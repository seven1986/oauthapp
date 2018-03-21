using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.Apis.Common;
using static IdentityServer4.MicroService.AppConstant;
using static IdentityServer4.MicroService.MicroserviceConfig;
using System.Net.Http;
using System.IO;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Models.Apis.CodeGenController;
using System;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// Swagger CodeGen
    /// </summary>
    [Route("CodeGen")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = Roles.Users)]
    public class CodeGenController : BasicController
    {
        #region Services
        readonly SwaggerCodeGenService swagerCodeGen;
        readonly INodeServices nodeServices;
        #endregion

        public CodeGenController(
            IStringLocalizer<CodeGenController> localizer,
            SwaggerCodeGenService _swagerCodeGen,
            INodeServices _nodeServices)
        {
            l = localizer;
            swagerCodeGen = _swagerCodeGen;
            nodeServices = _nodeServices;
        }

        /// <summary>
        /// Swagger CodeGen Clients
        /// </summary>
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
        [HttpGet("Servers")]
        [AllowAnonymous]
        [SwaggerOperation("CodeGen/Servers")]
        public ApiResult<List<SwaggerCodeGenItem>> Servers(bool fromCache = true)
        {
            var result = fromCache ? swagerCodeGen.ServerItemsCache : swagerCodeGen.ServerItems;

            return new ApiResult<List<SwaggerCodeGenItem>>(result);
        }


        /// <summary>
        /// Generate Client SDK
        /// </summary>
        [HttpPost("GenerateClient")]
        [AllowAnonymous]
        [SwaggerOperation("CodeGen/GenerateClient")]
        public async Task<ApiResult<string>> GenerateClient(GenerateClientRequest value)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResult<string>()
                {
                    code = (int)BasicControllerEnums.UnprocessableEntity,
                    message = ModelErrors()
                };
            }

            var tempPath = $"./Node/sdkgen/{Enum.GetName(typeof(PackagePlatform), value.platform)}/{value.templateKey}";

            if (!System.IO.File.Exists(tempPath))
            {
                return new ApiResult<string>(l, CodeGenControllerEnums.GenerateClient_ModuleIsNotExists);
            }

            try
            {
                var r = await nodeServices.InvokeAsync<string>(tempPath, value.packageOptions);

                return new ApiResult<string>(r);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l,
                    BasicControllerEnums.ExpectationFailed,
                    ex.Message);
            }
        }
    }
}
