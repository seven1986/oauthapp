using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.SwaggerGen;
using IdentityServer4.MicroService.Codes;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using static IdentityServer4.MicroService.AppConstant;

namespace IdentityServer4.MicroService.Apis
{
    [Route("File")]
    public class FileController : BasicController
    {
        #region Services
        // azure Storage
        readonly AzureStorageService azure;
        #endregion

        static string blobContainerName = "campaign-core-identity";

        public FileController(
            AzureStorageService _azure,
            IStringLocalizer<FileController> localizer)
        {
            azure = _azure;
            l = localizer;
        }

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

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Upload)]
        [SwaggerOperation("File/Post")]
        public async Task<ApiResult<string>> Post([FromForm]IFormFile value)
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
                    return new ApiResult<string>(l,BasicControllerEnums.UnprocessableEntity,
                        "视频应小于20MB");
                }
                //return new SingleResult<string>(StatusCodes.Status415UnsupportedMediaType,
                //    l["文件类型错误"]);
            }

            try
            {
                var result = await azure.UploadBlob(value.OpenReadStream(), blobContainerName, value.FileName);

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source);
            }
        }

        #region Image Settings
        static long ImageSizeLimit = 1024 * 1024 * 5;
        static string[] AllowedImageTypes = new string[] { "image/jpeg", "image/jpg", "image/png" }; 
        #endregion

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="value">图片文件</param>
        /// <returns></returns>
        [HttpPost("Image")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Upload)]
        [SwaggerOperation("File/Image")]
        public async Task<ApiResult<string>> Image([FromForm]IFormFile value)
        {
            if (value == null)
            {
                return new ApiResult<string>(l,BasicControllerEnums.UnprocessableEntity,
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
                var result = await azure.UploadBlob(value.OpenReadStream(), blobContainerName, value.FileName);

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed, ex.Message);
            }
        }
    }
}
