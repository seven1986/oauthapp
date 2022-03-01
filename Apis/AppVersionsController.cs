using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用发布版本")]
    public class AppVersionsController : BaseController
    {
        private readonly AppDbContext _context;

        public AppVersionsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{appId}")]
        [SwaggerOperation(OperationId = "AppVersionsList")]
        [EncryptResultFilter]
        public async Task<ActionResult<IEnumerable<AppVersion>>> GetAppVersion()
        {
            return await _context.AppVersions.Where(x => x.AppID == AppID)
                .OrderByDescending(x => x.ID).ToListAsync();
        }
    }
}
