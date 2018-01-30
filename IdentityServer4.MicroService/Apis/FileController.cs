using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.SwaggerGen;
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

        static string blobContainerName = "campaign-core-identity";

        public FileController(
            AzureStorageService _azure,
            IStringLocalizer<FileController> localizer)
        {
            azure = _azure;
            l = localizer;
        }

        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Upload)]
        [SwaggerOperation("File/Post")]
        public async Task<SingleResult<string>> Post([FromForm]IFormFile value)
        {
            if (value == null)
            {
                return new SingleResult<string>(StatusCodes.Status422UnprocessableEntity,
                    l["未发现任何文件"]);
            }

            if (AllowedVideoTypes.Any(b => b.Contains(value.ContentType)))
            {
                if (value.Length < 1 || value.Length > 20000000)
                {
                    return new SingleResult<string>(StatusCodes.Status413PayloadTooLarge,
                        l["视频应小于20MB"]);
                }
            }
            else if (AllowedDocTypes.Any(b => b.Contains(value.ContentType)))
            {
                if (value.Length < 1 || value.Length > 10000000)
                {
                    return new SingleResult<string>(StatusCodes.Status413PayloadTooLarge,
                        l["文档应小于10MB"]);
                }
            }
            else
            {
                // treats as video
                if (value.Length < 1 || value.Length > 20000000)
                {
                    return new SingleResult<string>(StatusCodes.Status413PayloadTooLarge,
                        l["视频应小于20MB"]);
                }
                //return new SingleResult<string>(StatusCodes.Status415UnsupportedMediaType,
                //    l["文件类型错误"]);
            }

            try
            {
                var result = await azure.UploadBlob(value.OpenReadStream(), blobContainerName, value.FileName);

                return new SingleResult<string>(result);
            }

            catch (Exception ex)
            {
                return new SingleResult<string>(StatusCodes.Status500InternalServerError, ex.Message + ex.Source);
            }
        }
    }
}
