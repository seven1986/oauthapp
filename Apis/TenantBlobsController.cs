using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Tenant;
using OAuthApp.Services;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("租户文件")]
    public class TenantBlobsController : BaseController
    {
        private readonly TenantDbContext _context;
        private readonly UploadService _uploader;
        private readonly TenantContext _tenant;

        public TenantBlobsController(
            TenantDbContext context,
            UploadService uploader,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _uploader = uploader;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "TenantBlobs")]
        [EncryptResultFilter]
        public IActionResult List(string channelCode,string channelAppId,string tag)
        {
            var q = _context.TenantBlobs.Where(x => x.TenantID == _tenant.Id);

            if(!string.IsNullOrWhiteSpace(channelCode))
            {
                q = q.Where(x => x.ChannelCode.Equals(channelCode));
            }

            if (!string.IsNullOrWhiteSpace(channelAppId))
            {
                q = q.Where(x => x.ChannelAppID.Equals(channelAppId));
            }

            if (!string.IsNullOrWhiteSpace(tag))
            {
                q = q.Where(x => x.Tag.Contains(tag));
            }

            var result = q.ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "TenantBlob")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result =  _context.TenantBlobs.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "TenantBlobPut")]
        public IActionResult Put(long id, TenantBlob tenantBlob)
        {
            if (id != tenantBlob.ID)
            {
                return BadRequest();
            }

            _context.Entry(tenantBlob).State = EntityState.Modified;

            try
            {
                 _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return OK(true);
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "TenantBlobPost")]
        public IActionResult Post(TenantBlob tenantBlob)
        {
            tenantBlob.TenantID = _tenant.Id;

            _context.TenantBlobs.Add(tenantBlob);

            _context.SaveChanges();

            return OK(new { id = tenantBlob.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "TenantBlobDelete")]
        public IActionResult Delete(long id)
        {
            var tenantBlob = _context.TenantBlobs.Find(id);

            if (tenantBlob == null)
            {
                return NotFound();
            }

            _context.TenantBlobs.Remove(tenantBlob);

            _context.SaveChanges();

            return OK(true);
        }

        [HttpPost("Upload")]
        [SwaggerOperation(OperationId = "TenantBlobUpload")]
        public IActionResult Upload([FromQuery] string channelCode, [FromQuery] string channelAppId, IFormFile file)
        {
            var savePath = $"{_tenant.Id}/{channelCode}/{channelAppId}/" + file.FileName;

            _uploader.Upload(savePath, file);

            #region 累计应用文件用量
            _context.TenantOrders.Add(new TenantOrder()
            {
                Amount = file.Length,
                ChannelAppID = channelAppId,
                ChannelCode = ChannelCodes.AppBlob,
                TenantID = _tenant.Id
            });
            _context.SaveChanges();
            #endregion

            return OK(AppConst.BlobServer + "/" + savePath);
        }
    }
}
