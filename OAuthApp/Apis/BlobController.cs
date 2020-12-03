using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using OAuthApp.Enums;
using OAuthApp.Services;
using OAuthApp.Models.Apis.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.IO;
using static OAuthApp.AppConstant;

namespace OAuthApp.Apis
{
    /// <summary>
    /// Blob
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    [Authorize(AuthenticationSchemes = AppAuthenScheme, Roles = DefaultRoles.User)]
    [ApiExplorerSettingsDynamic("Blob")]
    [SwaggerTag("#### 文件上传服务，需要开通Azure Storage才能使用")]
    public class BlobController : ApiControllerBase
    {
        #region Services
        // azure Storage
        readonly AzureStorageService azure;
        #endregion

        #region 构造函数
        public BlobController(
            AzureStorageService _azure,
            IStringLocalizer<BlobController> localizer)
        {
            azure = _azure;
            l = localizer;
        }
        #endregion

        #region Blob - 上传文件

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
        /// Blob - 上传文件
        /// </summary>
        /// <param name="value">视频(小于20MB),文档文件(小于10MB)</param>
        /// <param name="folderName">文件夹名称,5~30个字节，英文或数字组装成。默认为当前日期yyyyMMdd</param>
        /// <returns></returns>
        [HttpPost("File")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:blob.file")]
        [SwaggerOperation(
            OperationId = "BlobFile", 
            Summary = "Blob - 上传文件",
            Description = "视频支持：avi,quicktime,asf,wmv,flv,matroska,mp4,webm,wmv,flash,mpeg。文档支持：pdf,word,excel。\r\n\r\n #### 需要权限 \r\n " + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.blob.file | oauthapp.blob.file |")]
        public ApiResult<string> File(IFormFile value, [FromQuery][RegularExpression("[a-zA-Z0-9]{5,30}")]string folderName)
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
            }

            try
            {
                var Guid_FileName = Guid.NewGuid().ToString("N") + Path.GetExtension(value.FileName).ToLower();

                if (string.IsNullOrWhiteSpace(folderName))
                {
                    folderName = DateTime.UtcNow.ToString("yyyyMMdd");
                }

                var result = azure.UploadBlobAsync(value.OpenReadStream(), folderName, Guid_FileName).Result;

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed, ex.Message + ex.Source);
            }
        }
        #endregion

        #region Blob - 上传图片

        #region Image Settings
        static long ImageSizeLimit = 1024 * 1024 * 10;
        static string[] AllowedImageTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "application/octet-stream" };
        #endregion

        /// <summary>
        /// Blob - 上传图片
        /// </summary>
        /// <param name="value">图片文件</param>
        /// <param name="folderName">文件夹名称,5~30个字节，英文或数字组装成。默认为当前日期yyyyMMdd</param>
        /// <returns></returns>
        [HttpPost("Image")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:blob.image")]
        [SwaggerOperation(OperationId = "BlobImage",Summary = "Blob - Image",Description = "支持图片：jpeg,jpg,png,octet-stream，小于10MB。\r\n\r\n #### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.blob.image | oauthapp.blob.image |")]
        public ApiResult<string> UploadImage([FromForm]IFormFile value, [FromQuery][RegularExpression("[a-zA-Z0-9]{5,30}")]string folderName)
        {
            if (value == null)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "未发现任何文件");
            }

            if (!AllowedImageTypes.Any(b => b.Contains(value.ContentType)))
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "文件类型必须是.jpg/.jpeg/.png/octet-stream,error:" + value.ContentType);
            }

            if (value.Length < 1 || value.Length > ImageSizeLimit)
            {
                return new ApiResult<string>(l, BasicControllerEnums.UnprocessableEntity,
                    "图片应小于10MB");
            }

            try
            {
                var file_ContentType = value.ContentType.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1].ToLower();

                if (file_ContentType.Equals("octet-stream"))
                {
                    file_ContentType = "png";
                }

                var Guid_FileName = Guid.NewGuid().ToString("N") + "." + file_ContentType;

                if (string.IsNullOrWhiteSpace(folderName))
                {
                    folderName = DateTime.UtcNow.ToString("yyyyMMdd");
                }

                var result = azure.UploadBlobAsync(value.OpenReadStream(), folderName,Guid_FileName).Result;

                return new ApiResult<string>(result);
            }

            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.ExpectationFailed, ex.Message);
            }
        }
        #endregion

        #region Blob - 上传图片（Base64格式）
        /// <summary>
        /// Blob - Base64
        /// </summary>
        /// <param name="value">base64字符串</param>
        /// <param name="folderName">文件夹名称,5~30个字节，英文或数字组装成。默认为当前日期yyyyMMdd</param>
        /// <returns></returns>
        [HttpPost("Base64")]
        [Authorize(AuthenticationSchemes = AppAuthenScheme, Policy = "scope:blob.base64")]
        [SwaggerOperation(
            OperationId = "BlobBase64", 
            Summary = "Blob - 上传图片（Base64格式）",
            Description = "上传Base64格式的png图片。\r\n\r\n #### 需要权限\r\n" + "| client scope | user permission |\r\n" + "| ---- | ---- |\r\n" + "| oauthapp.blob.base64 | oauthapp.blob.base64 |")]
        public ApiResult<string> Base64([FromBody]string value, [FromQuery][RegularExpression("[a-zA-Z0-9]{5,30}")]string folderName)
        {
            try
            {
                var byteBuffer = Convert.FromBase64String(value);

                var memoryStream = new MemoryStream(byteBuffer);

                var Guid_FileName = Guid.NewGuid().ToString("N") + ".png";

                if (string.IsNullOrWhiteSpace(folderName))
                {
                    folderName = DateTime.UtcNow.ToString("yyyyMMdd");
                }

                var drawingUrl = azure.UploadBlobAsync(memoryStream,folderName, Guid_FileName).Result;

                return new ApiResult<string>(drawingUrl);
            }
            catch (Exception ex)
            {
                return new ApiResult<string>(l, BasicControllerEnums.HasError, ex.Message);
            }
        }
        #endregion

        #region Blob - 错误码表
        /// <summary>
        /// Blob - 错误码表
        /// </summary>
        /// <returns></returns>
        [HttpGet("Codes")]
        [AllowAnonymous]
        [SwaggerOperation(
            OperationId = "BlobCodes",
            Summary = "Blob - 错误码表",
            Description = "Blob错误码对照表")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public List<ApiCodeModel> Codes()
        {
            var result = _Codes<FileControllerEnums>();

            return result;
        }
        #endregion
    }
}
