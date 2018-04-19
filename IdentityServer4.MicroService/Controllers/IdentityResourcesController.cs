using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.IdentityResourceModels;

namespace IdentityServer4.MicroService.Controllers
{
    public class IdentityResourcesController : Controller
    {
        private readonly ConfigurationDbContext _context;

        public IdentityResourcesController(ConfigurationDbContext context)
        {
            _context = context;    
        }

        // GET: IdentityResource
        public async Task<IActionResult> Index([FromQuery]PagingRequest value)
        {
            var resources = await _context.IdentityResources.Skip(value.skip).Take(value.take)
              .Include(x => x.UserClaims)
              .AsNoTracking()
              .ToListAsync();

            var total = await _context.IdentityResources.CountAsync();

            var model = new GetResponse()
            {
                resources = resources,
                total = total
            };

            return View(model);
        }

        // GET: IdentityResource/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.IdentityResources
                .Include(x=>x.UserClaims)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // GET: IdentityResource/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: IdentityResource/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityResource idResource)
        {
            var claims = DeserializeFormData<IdentityClaim>("UserClaims");

            if (claims!=null&&claims.Count > 0)
            {
                idResource.UserClaims = claims;
            }

            if (ModelState.IsValid)
            {
                _context.Add(idResource);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(idResource);
        }

        // GET: IdentityResource/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.IdentityResources
                .Include(x => x.UserClaims)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: IdentityResource/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, IdentityResource idResource)
        {
            var src = await _context.IdentityResources
             .Include(x => x.UserClaims)
             .AsNoTracking()
             .SingleOrDefaultAsync(m => m.Id == id);

            #region Claims
            List<IdentityClaim>
                Claims_Add = null,
                Claims_Update = null,
                Claims_Delete = null;
            var claims = DeserializeFormData<IdentityClaim>("UserClaims");
            if (claims.Count > 0)
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

            #region Submit to database
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(idResource);
                    await _context.SaveChangesAsync();

                    #region Claims
                    if (Claims_Add.Count > 0)
                    {
                        Claims_Add.ForEach(x => x.IdentityResource = idResource);
                        _context.AddRange(Claims_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Claims_Update.Count > 0)
                    {
                        Claims_Update.ForEach(x => x.IdentityResource = idResource);
                        _context.UpdateRange(Claims_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Claims_Delete.Count > 0)
                    {
                        Claims_Delete.ForEach(x => x.IdentityResource = null);
                        _context.RemoveRange(Claims_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!Exists(id))
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

            return View(idResource);
        }

        // GET: IdentityResource/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entity = await _context.IdentityResources
               .Include(x => x.UserClaims)
               .SingleOrDefaultAsync(m => m.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: IdentityResource/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var appRole = await _context.IdentityResources.SingleOrDefaultAsync(m => m.Id == id);
            _context.IdentityResources.Remove(appRole);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool Exists(long id)
        {
            return _context.IdentityResources.Any(e => e.Id == id);
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
