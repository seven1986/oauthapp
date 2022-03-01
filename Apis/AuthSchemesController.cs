using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Tenant;
using Swashbuckle.AspNetCore.Annotations;

namespace OAuthApp.Apis
{
    [SwaggerTag("授权协议")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AuthSchemesController : BaseController
    {
        private readonly TenantDbContext _context;

        public AuthSchemesController(TenantDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "AuthSchemes")]
        public IActionResult List()
        {
            var result = _context.AuthSchemes
                .Where(x => !x.IsDelete && x.UserID == UserID)
                .OrderByDescending(x => x.ID).ToList();

            return OK(result);
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "AuthScheme")]
        public IActionResult Get(long id)
        {
            var result = _context.AuthSchemes
                .FirstOrDefault(x => x.ID == id && x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "AuthSchemePut")]
        public IActionResult Put(long id, AuthScheme authScheme)
        {
            if (UserID != authScheme.UserID)
            {
                return NotFound();
            }

            _context.Entry(authScheme).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "AuthSchemePost")]
        public IActionResult Post(AuthScheme authScheme)
        {
            authScheme.UserID = UserID;

            _context.AuthSchemes.Add(authScheme);

            _context.SaveChanges();

            return OK(new { id = authScheme.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "AuthSchemeDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.AuthSchemes
              .FirstOrDefault(x => x.ID == id && x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            _context.AuthSchemes.Remove(result);
            _context.SaveChanges();

            return OK(true);
        }
    }
}
