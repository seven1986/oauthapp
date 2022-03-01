using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System;
using OAuthApp.Tenant;
using OAuthApp.Filters;
using Microsoft.AspNetCore.Http;

namespace OAuthApp.Apis
{
    [SwaggerTag("属性模板")]
    public class PropertyTemplatesController : BaseController
    {
        private readonly TenantDbContext _context;
        private readonly TenantContext _tenant;


        public PropertyTemplatesController(TenantDbContext context,
            IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
        }

        [HttpGet("Market")]
        [SwaggerOperation(OperationId = "PropertyTemplateMarket")]
        [EncryptResultFilter]
        public IActionResult Market(string channelCode)
        {
            var q = _context.PropertyTemplates
               .Where(x => !x.IsDelete && x.Show);

            if(!string.IsNullOrWhiteSpace(channelCode))
            {
                q = q.Where(x => x.ChannelCode.Equals(channelCode));
            }

            var result = q.OrderByDescending(x => x.ID).ToList();

            return OK(result);
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "PropertyTemplates")]
        [EncryptResultFilter]
        public IActionResult List([Required][FromQuery] string channelCode)
        {
            var result = _context.PropertyTemplates
                .Where(x => x.ChannelCode.Equals(channelCode) &&
                 (x.TenantID == 0 || x.TenantID == _tenant.Id) &&
                 !x.IsDelete && x.Show).OrderByDescending(x => x.ID).ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "PropertyTemplate")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.PropertyTemplates.FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "PropertyTemplatePut")]
        public IActionResult Put(long id, PropertyTemplate propertyTemplate)
        {
            propertyTemplate.TenantID = _tenant.Id;

            if (id != propertyTemplate.ID)
            {
                return BadRequest();
            }

            if(propertyTemplate.IsSystem)
            {
                return Error("无法修改系统默认配置。");
            }

            _context.Entry(propertyTemplate).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "PropertyTemplatePost")]
        public IActionResult Post(PropertyTemplate propertyTemplate)
        {
            propertyTemplate.TenantID = _tenant.Id;
            propertyTemplate.IsSystem = false;
            _context.PropertyTemplates.Add(propertyTemplate);

            _context.SaveChanges();

            return OK(new { id = propertyTemplate.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "PropertyTemplateDelete")]
        public IActionResult Delete(long id)
        {
            var propertyTemplate = _context.PropertyTemplates.FirstOrDefault(x => x.ID == id &&
             x.TenantID == _tenant.Id);

            if (propertyTemplate == null)
            {
                return NotFound();
            }

            if (propertyTemplate.IsSystem)
            {
                return Error("无法删除系统默认配置。");
            }

            _context.PropertyTemplates.Remove(propertyTemplate);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
