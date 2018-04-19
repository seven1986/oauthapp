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
    [Route("Image")]
    public class ImageController : BasicController
    {
        #region Services
        readonly AzureStorageService azure;
        static long ImageSizeLimit = 1024 * 1024 * 5;
        static string[] AllowedImageTypes = new string[] { "image/jpeg", "image/jpg", "image/png" };
        static string blobContainerName = "campaign-core-identity";
        #endregion

        public ImageController(
            AzureStorageService _azure,
            IStringLocalizer<ImageController> localizer)
        {
            azure = _azure;
            l = localizer;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="value">图片文件</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = ClientScopes.Upload)]
        [SwaggerOperation("Image/Post")]
        public async Task<SingleResult<string>> Post([FromForm]IFormFile value)
        {
            if (value == null)
            {
                return new SingleResult<string>(StatusCodes.Status422UnprocessableEntity,
                    l["未发现任何文件"]);
            }

            if (!AllowedImageTypes.Any(b => b.Contains(value.ContentType)))
            {
                return new SingleResult<string>(StatusCodes.Status415UnsupportedMediaType, 
                    l["文件类型必须是.jpg/.jpeg/.png"]);
            }

            if (value.Length < 1 || value.Length > ImageSizeLimit)
            {
                return new SingleResult<string>(StatusCodes.Status413PayloadTooLarge,
                    l["图片应小于5MB"]);
            }

            try
            {
                var result = await this.azure.UploadBlob(value.OpenReadStream(), blobContainerName, value.FileName);

                return new SingleResult<string>(result);
            }

            catch (Exception ex)
            {
                return new SingleResult<string>(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
