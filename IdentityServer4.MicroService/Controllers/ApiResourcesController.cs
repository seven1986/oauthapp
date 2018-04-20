using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.ApiResourceModels;

namespace IdentityServer4.MicroService.Controllers
{
    [Authorize(Roles = AppConstant.Roles.Administrators)]
    public class ApiResourcesController : Controller
    {
        private readonly ConfigurationDbContext _context;
        private readonly SqlService _sql;

        public ApiResourcesController(ConfigurationDbContext context,
            SqlService sql)
        {
            _context = context;
            _sql = sql;
        }

        // GET: AppUsers
        public async Task<IActionResult> Index([FromQuery]PagingRequest value)
        {
            var apis = await _context.ApiResources.Skip(value.skip).Take(value.take)
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .Include(x => x.Scopes)
                .AsNoTracking()
                .ToListAsync();

            var total = await _context.ApiResources.CountAsync();

            var model = new GetResponse()
            {
                apis = apis,
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

            var api = await _context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .Include(x => x.Scopes)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (api == null)
            {
                return NotFound();
            }

            return View(api);
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
        public async Task<IActionResult> Create(ApiResource value)
        {
            if (ModelState.IsValid)
            {
                _context.Add(value);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(value);
        }

        // GET: AppUsers/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var api = await _context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .Include(x => x.Scopes)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            if (api == null)
            {
                return NotFound();
            }

            return View(api);
        }

        // POST: AppUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, ApiResource apiResource)
        {
            var src = await _context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.UserClaims)
                .Include(x => x.Scopes)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id);

            #region Claims
            List<ApiResourceClaim>
                Claims_Add = null,
                Claims_Update = null,
                Claims_Delete = null;
            var claims = DeserializeFormData<ApiResourceClaim>("UserClaims");
            if (claims!=null && claims.Count > 0)
            {
                Claims_Add = claims.Where(x => x.Id == 0).ToList();
                Claims_Update = claims.Where(x => x.Id > 0).ToList();
                var Ids = claims.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    Claims_Delete = src.UserClaims.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region AllowedScopes
            List<ApiScope>
                Scopes_Add = null,
                Scopes_Update = null,
                Scopes_Delete = null;
            var scopes = DeserializeFormData<ApiScope>("Scopes");
            if (scopes!=null && scopes.Count > 0)
            {
                Scopes_Add = scopes.Where(x => x.Id == 0).ToList();
                Scopes_Update = scopes.Where(x => x.Id > 0).ToList();
                var Ids = scopes.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    Scopes_Delete = src.Scopes.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region ClientSecrets
            List<ApiSecret>
                Secrets_Add = null,
                Secrets_Update = null,
                Secrets_Delete = null;
            var secrets = DeserializeFormData<ApiSecret>("Secrets");
            if (secrets!=null && secrets.Count > 0)
            {
                Secrets_Add = secrets.Where(x => x.Id == 0).ToList();
                Secrets_Update = secrets.Where(x => x.Id > 0).ToList();
                var Ids = secrets.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    Secrets_Delete = src.Secrets.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region Submit to database
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(apiResource);
                    await _context.SaveChangesAsync();

                    #region Claims
                    if (Claims_Add!=null&&Claims_Add.Count > 0)
                    {
                        Claims_Add.ForEach(x => x.ApiResource = apiResource);
                        _context.AddRange(Claims_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Claims_Update != null && Claims_Update.Count > 0)
                    {
                        Claims_Update.ForEach(x => x.ApiResource = apiResource);
                        _context.UpdateRange(Claims_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Claims_Delete != null && Claims_Delete.Count > 0)
                    {
                        Claims_Delete.ForEach(x => x.ApiResource = null);
                        _context.RemoveRange(Claims_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region AllowedScopes
                    if (Scopes_Add!=null&&Scopes_Add.Count > 0)
                    {
                        Scopes_Add.ForEach(x => x.ApiResource = apiResource);
                        _context.AddRange(Scopes_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Scopes_Update != null && Scopes_Update.Count > 0)
                    {
                        Scopes_Update.ForEach(x => x.ApiResource = apiResource);
                        _context.UpdateRange(Scopes_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Scopes_Delete != null && Scopes_Delete.Count > 0)
                    {
                        Scopes_Delete.ForEach(x => x.ApiResource = null);
                        _context.RemoveRange(Scopes_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region ClientSecrets
                    if (Secrets_Add != null && Secrets_Add.Count > 0)
                    {
                        Secrets_Add.ForEach(x => x.ApiResource = apiResource);
                        _context.AddRange(Secrets_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Secrets_Update != null && Secrets_Update.Count > 0)
                    {
                        Secrets_Update.ForEach(x => x.ApiResource = apiResource);
                        _context.UpdateRange(Secrets_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Secrets_Delete != null && Secrets_Delete.Count > 0)
                    {
                        Secrets_Delete.ForEach(x => x.ApiResource = null);
                        _context.RemoveRange(Secrets_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserExists(id))
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
            #endregion

            return View(apiResource);
        }

        // GET: AppUsers/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUser = await _context.ApiResources
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
            var appUser = await _context.ApiResources.SingleOrDefaultAsync(m => m.Id == id);
            _context.ApiResources.Remove(appUser);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool AppUserExists(long id)
        {
            return _context.ApiResources.Any(e => e.Id == id);
        }

        #region helper
        List<T> DeserializeFormData<T>(string formKey)
        {
            var formString = Request.Form[formKey].ToString();
            if (!string.IsNullOrWhiteSpace(formString))
            {
                var result = JsonConvert.DeserializeObject<List<T>>(formString);
                if (result != null && result.Count > 0)
                {
                    return result;
                }
            }
            return default(List<T>);
        }
        #endregion
    }
}
