using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("用户服务器")]
    public class UserAppServersController : BaseController
    {
        private readonly AppDbContext _context;

        public UserAppServersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "UserAppServers")]
        [EncryptResultFilter]
        public IActionResult List()
        {
            var result = _context.UserAppServers.Where(x => x.UserID == UserID).ToList();

            return OK(result);
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "UserAppServerPost")]
        public IActionResult Post(UserAppServer userAppServer)
        {
            var result = _context.UserAppServers
                .Where(x => x.UserID == UserID && x.AppServerID == userAppServer.AppServerID).FirstOrDefault();
            if (result == null)
            {
                userAppServer.UserID = UserID;

                _context.UserAppServers.Add(userAppServer);

                _context.SaveChanges();

                return OK(new { id = userAppServer.ID });
            }
            else
            {
                return OK(new { id = result.ID });
            }
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "UserAppServerDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.UserAppServers
                .FirstOrDefault(x => x.ID == id && x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            _context.UserAppServers.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
