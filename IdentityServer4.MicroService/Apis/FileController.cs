using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using IdentityServer4.MicroService.Enums;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.Apis.Common;
using Swashbuckle.AspNetCore.Annotations;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    /// <summary>
    /// 文件
    /// </summary>
    [Route("File")]
    [Produces("application/json")]
    [ApiExplorerSettingsDynamic("File")]
    public class FileController : ApiControllerBase
    {
        #region Services
        // azure Storage
        readonly AzureStorageService azure;
        #endregion

        static string blobContainerName = "identityserver4-microservice";

        #region 构造函数
        public FileController(
            AzureStorageService _azure,
            IStringLocalizer<FileController> localizer)
        {
            azure = _azure;
            l = localizer;
        } 
        #endregion

        #region File Settings
        // AllowedVideoTypes
        static string[] AllowedVideoTypes = new string[] {
            "video/avi",
            "video/quicktime",
            "video/asf",
            "video/wmv",
            "video/x-flv",
            "video/flv",
            "video/x-matroska",
            "video/mp4",
            "video/webm",
            "video/x-ms-wmv",
            "application/x-shockwave-flash",
            "video/mpeg"
        };
        // AllowedDocTypes
        static string[] AllowedDocTypes = new string[] {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
        };
        #endregion

        #region 文件 - 上传视频或文档
        /// <summary>
        /// 文件 - 上传视频或文档
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <remarks>
        /// Scope：isms.file.post
        /// </remarks>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:file.post")]
        [SwaggerOperation(OperationId = "FilePost")]
        public async Task<ApiResult<string>> Post(IFormFile value)
        {
            if (value == null)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "未发现任何文件");
            }

            if (AllowedVideoTypes.Any(b => b.Contains(value.ContentType)))
            {
                if (value.Length < 1 || value.Length > 20000000)
                {
                    return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                        "视频应小于20MB");
                }
            }

            else if (AllowedDocTypes.Any(b => b.Contains(value.ContentType)))
            {
                if (value.Length < 1 || value.Length > 10000000)
                {
                    return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                        "文档应小于10MB");
                }
            }
            else
            {
                // treats as video
                if (value.Length < 1 || value.Length > 20000000)
                {
                    return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                        "视频应小于20MB");
                }
                //return new SingleResult<string>(StatusCodes.Status415UnsupportedMediaType,
                //    l["文件类型错误"]);
            }

            try
            {
                var result = await azure.UploadBlobAsync(value.OpenReadStream(), blobContainerName, value.FileName);

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source);
            }
        } 
        #endregion

        #region Image Settings
        static long ImageSizeLimit = 1024 * 1024 * 5;
        static string[] AllowedImageTypes = new string[] { "image/jpeg", "image/jpg", "image/png" };
        #endregion

        #region 文件 - 上传图片
        /// <summary>
        /// 文件 - 上传图片
        /// </summary>
        /// <param name="value">图片文件</param>
        /// <returns></returns>
        /// <remarks>
        /// Scope：isms.file.image
        /// </remarks>
        [HttpPost("Image")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:file.image")]
        [SwaggerOperation(OperationId = "FileImage")]
        public async Task<ApiResult<string>> Image(IFormFile value)
        {
            if (value == null)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "未发现任何文件");
            }

            if (!AllowedImageTypes.Any(b => b.Contains(value.ContentType)))
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "文件类型必须是.jpg/.jpeg/.png");
            }

            if (value.Length < 1 || value.Length > ImageSizeLimit)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "图片应小于5MB");
            }

            try
            {
                var result = await azure.UploadBlobAsync(value.OpenReadStream(), blobContainerName, value.FileName);

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed, ex.Message);
            }
        } 
        #endregion

        #region 文件 - 错误码表
        /// <summary>
        /// 文件 - 错误码表
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 文件代码对照表
        /// </remarks>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "FileCodes")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<FileControllerEnums>();

            return result;
        }
        #endregion
    }
}
