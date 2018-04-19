using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.RoleModels;

namespace IdentityServer4.MicroService.Controllers
{
    public class AppRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SqlService _sql;

        public AppRolesController(ApplicationDbContext context, SqlService sql)
        {
            _context = context;
            _sql = sql;
        }

        // GET: AppRoles
        public async Task<IActionResult> Index([FromQuery]PagingRequest value)
        {
            var roles = await _context.Roles.Skip(value.skip).Take(value.take)
               .Include(x => x.Claims)
               .AsNoTracking()
               .ToListAsync();

            var total = await _context.Roles.CountAsync();

            var model = new GetResponse()
            {
                roles = roles,
                total = total
            };

            return View(model);
        }

        // GET: AppRoles/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appRole = await _context.Roles
                //.Include(x=>x.Claims)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (appRole == null)
            {
                return NotFound();
            }

            return View(appRole);
        }

        // GET: AppRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AppRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,NormalizedName,ConcurrencyStamp")] AppRole appRole)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appRole);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(appRole);
        }

        // GET: AppRoles/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appRole = await _context.Roles
                // .Include(x=>x.Claims)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (appRole == null)
            {
                return NotFound();
            }
            return View(appRole);
        }

        // POST: AppRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, AppRole appRole)
        {
            if (id != appRole.Id)
            {
                return NotFound();
            }

            #region claims
            List<AppRoleClaim> Claims_Update = null, Claims_Delete = null, Claims_Add = null;
            var claimsStr = Request.Form["Claims"].ToString();
            if (!string.IsNullOrWhiteSpace(claimsStr))
            {
                var claims = JsonConvert.DeserializeObject<List<AppRoleClaim>>(claimsStr);
                if (claims != null && claims.Count > 0)
                {
                    claims.ForEach(x => { x.RoleId = id; });
                    Claims_Update = claims.Where(x => x.Id > 0).ToList();
                    Claims_Add = claims.Where(x => x.Id == 0).ToList();
                    var Ids = claims.Select(x => x.Id).ToList();
                    if (Ids.Count > 0)
                    {
                        Claims_Delete = _context.RoleClaims.Where(x => x.RoleId == id && !Ids.Contains(x.Id)).ToList();
                    }
                }
            }
            #endregion

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appRole);

                    if (Claims_Add != null && Claims_Add.Count > 0)
                        _context.AddRange(Claims_Add);

                    if (Claims_Update != null && Claims_Update.Count > 0)
                        _context.UpdateRange(Claims_Update);

                    if (Claims_Delete != null && Claims_Delete.Count > 0)
                        _context.RemoveRange(Claims_Delete);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppRoleExists(appRole.Id))
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
            return View(appRole);
        }

        // GET: AppRoles/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appRole = await _context.Roles
                .SingleOrDefaultAsync(m => m.Id == id);
            if (appRole == null)
            {
                return NotFound();
            }

            return View(appRole);
        }

        // POST: AppRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var appRole = await _context.Roles.SingleOrDefaultAsync(m => m.Id == id);
            _context.Roles.Remove(appRole);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AppRoleExists(long id)
        {
            return _context.Roles.Any(e => e.Id == id);
        }
    }
}
