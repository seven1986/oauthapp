using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.AppUsersModels;

namespace IdentityServer4.MicroService.Controllers
{
    [Authorize]
    //(Roles = AppConstant.Roles.Administrators)]
    public class AppUsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SqlService _sql;

        public AppUsersController(ApplicationDbContext context,
            SqlService sql)
        {
            _context = context;
            _sql = sql;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index([FromQuery]PagingRequest value)
        {
            var users = await
                _context.Users.Take(value.take)
                .Include("Logins")
                .Include("Claims")
                .Include("Roles")
                .AsNoTracking()
                .ToListAsync();

            var roles = await _context.Roles.ToDictionaryAsync(k => k.Id, v => v.NormalizedName);

            var total = await _context.Users.CountAsync();

            var model = new GetResponse()
            {
                roles = roles,
                users = users,
                total = total
            };

            return View(model);
        }

        // GET: AppUsers/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // GET: AppUsers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParentUserID,LineageIDs,Avatar,Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount")] AppUser appUser)
        {
            if (ModelState.IsValid)
            {
                // _context.Add(appUser.Claims);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .Include(x => x.Logins)
                .Include(x => x.Claims)
                .Include(x => x.Roles)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            if (appUser == null)
            {
                return NotFound();
            }

            var roles = await _context.Roles.Select(x => new { id = x.Id, name = x.Name }).ToListAsync();
            ViewData["roles"] = roles;

            return View(appUser);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AppUser appUser)
        {
            if (id != appUser.Id)
            {
                return NotFound();
            }

            #region claims
            List<AppUserClaim> Claims_Update = null, Claims_Delete = null, Claims_Add = null;
            var claimsStr = Request.Form["Claims"].ToString();
            if (!string.IsNullOrWhiteSpace(claimsStr))
            {
                var claims = JsonConvert.DeserializeObject<List<AppUserClaim>>(claimsStr);
                if (claims != null && claims.Count > 0)
                {
                    claims.ForEach(x => { x.UserId = id; });
                    Claims_Update = claims.Where(x => x.Id > 0).ToList();
                    Claims_Add = claims.Where(x => x.Id == 0).ToList();
                    var Ids = claims.Select(x => x.Id).ToList();
                    if (Ids.Count > 0)
                    {
                        Claims_Delete = _context.UserClaims.Where(x => x.UserId == id && !Ids.Contains(x.Id)).ToList();
                    }
                }
            }
            #endregion

            #region roles
            List<IdentityUserRole<long>> Roles_Add = null;
            var rolesStr = Request.Form["Roles"].ToString();
            if (!string.IsNullOrWhiteSpace(rolesStr))
            {
                var roles = JsonConvert.DeserializeObject<List<IdentityUserRole<long>>>(rolesStr);
                if (roles != null && roles.Count > 0)
                {
                    roles.ForEach(x => { x.UserId = id; });
                    Roles_Add = roles.ToList();
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUser);

                    if (Claims_Add != null && Claims_Add.Count > 0)
                        _context.AddRange(Claims_Add);
                    if (Claims_Update != null && Claims_Update.Count > 0)
                        _context.UpdateRange(Claims_Update);

                    if (Claims_Delete != null && Claims_Delete.Count > 0)
                        _context.RemoveRange(Claims_Delete);

                    await _context.SaveChangesAsync();

                    if (Roles_Add != null && Roles_Add.Count > 0)
                    {
                        var role = _context.UserRoles.Where(x => x.UserId == id).ToList();
                        _context.UserRoles.RemoveRange(role);

                        await _context.SaveChangesAsync();

                        _context.AddRange(Roles_Add);

                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(appUser.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(appUser);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.Users
                .SingleOrDefaultAsync(m => m.Id == id);
            if (appUser == null)
            {
                return NotFound();
            }

            return View(appUser);
        }

        // POST: AppUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var appUser = await _context.Users.SingleOrDefaultAsync(m => m.Id == id);
            _context.Users.Remove(appUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AppUserExists(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
