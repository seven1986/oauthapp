using OAuthApp.ApiModels.UserController;
using OAuthApp.Data;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using System;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("用户")]
    public class UserController : BaseController
    {
        #region services
        private readonly AppDbContext _context;
        //private readonly TokenProvider _tokenProvider;
        //private readonly ITenantUser _tenantUser;
        //private readonly TenantContext _tenant;
        #endregion

        #region construct
        public UserController(
            AppDbContext context
            //TokenProvider tokenProvider,
            //ITenantUser tenantUser,
            //IHttpContextAccessor contextAccessor
            )
        {
            _context = context;
            //_tokenProvider = tokenProvider;
            //_tenantUser = tenantUser;
            //_tenant = contextAccessor.HttpContext.GetTenantContext();
        }
        #endregion

        [HttpGet]
        [SwaggerOperation(OperationId = "Users")]
        [EncryptResultFilter]
        public IActionResult List(string userName, string phone, string email,
            int skip, int take)
        {
            var q = _context.Users.AsQueryable();

            if(!string.IsNullOrWhiteSpace(userName))
            {
                q = q.Where(x => x.UserName.Contains(userName));
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                q = q.Where(x => x.Phone.Contains(phone));
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                q = q.Where(x => x.Email.Contains(email));
            }

            var total = q.Count();

            var data = q.OrderByDescending(x => x.ID).Skip(skip).Take(take).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "User")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            var oauthusers = _context.Query<AppUserItem>(@"SELECT " +
                " (select Name from Apps t where A.AppID = t.ID ) AS AppName," +
                " (select Logo from Apps t where A.AppID = t.ID ) AS AppLogo," +
                " A.* FROM AppUsers A WHERE UserID = " + id);

            return OK(new { user, oauthusers });
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "UserPut")]
        public IActionResult Put(long id, User app)
        {
            if (id != app.ID)
            {
                return NotFound();
            }

            _context.Entry(app).State = EntityState.Modified;

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

        //[HttpDelete("{id}")]
        //[SwaggerOperation(OperationId = "UserDelete")]
        //public IActionResult Delete(long id)
        //{
        //    var user = _context.Users.Find(id);

        //    if (user == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Execute("DELETE FROM AppUser WHERE UserID = " + id);

        //    _context.Users.Remove(user);

        //    _context.SaveChanges();

        //    return OK(true);
        //}

        #region 账号注册
        [HttpPost("SignUp")]
        [SwaggerOperation(OperationId = "UserSignUp")]
        [AllowAnonymous]
        public IActionResult SignUp(UserSignUpRequest value)
        {
            if (_context.Users.Any(x => x.Email == value.Email))
            {
                return Error("账号已存在");
            }

            _context.Users.Add(new User()
            {
                Email = value.Email,
                Avatar = "",
                NickName = "",
                Password = value.Pwd,
                Phone = value.Phone,
                UserName = value.Email,
            });

            _context.SaveChanges();

            return OK(true);
        }
        #endregion
    }
}