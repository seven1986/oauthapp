using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用属性")]
    public class PropertySettingsController : BaseController
    {
        private readonly AppDbContext _context;

        public PropertySettingsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "PropertySettings")]
        [EncryptResultFilter]
        public IActionResult List([Required]string channelCode,long channelAppId)
        {
            var result = _context.PropertySettings
                .Where(x => x.ChannelCode == channelCode && x.ChannelAppId == channelAppId).ToList();

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "PropertySettingPut")]
        public IActionResult Put(long id, PropertySetting appProperty)
        {
           _context.Entry(appProperty).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "PropertySettingPost")]
        public IActionResult Post(PropertySetting appProperty)
        {
            _context.PropertySettings.Add(appProperty);

            _context.SaveChanges();

            return OK(new { id = appProperty.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "PropertySettingDelete")]
        public IActionResult Delete(long id)
        {
            var appProperty = _context.PropertySettings.FirstOrDefault(x => x.ID == id);

            if (appProperty == null)
            {
                return NotFound();
            }

            _context.PropertySettings.Remove(appProperty);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
