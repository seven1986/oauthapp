using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.MicroService.Data;
using IdentityServer4.MicroService.Services;
using IdentityServer4.MicroService.Models.CommonModels;
using IdentityServer4.MicroService.Models.ClientModels;

namespace IdentityServer4.MicroService.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ConfigurationDbContext _context;
        private readonly ApplicationDbContext _userContext;
        private readonly SqlService _sql;

        long UserId
        {
            get
            {
                return long.Parse(User.Claims.FirstOrDefault(x => x.Type.Equals("sub")).Value);
            }
        }

        public ClientsController(ConfigurationDbContext context,
            ApplicationDbContext userContext,
           SqlService sql)
        {
            _userContext = userContext;
            _context = context;
            _sql = sql;
        }

        // GET: Client
        public async Task<IActionResult> Index([FromQuery]PagingRequest value)
        {
            var ClientIDs = _userContext.UserClients.Where(x => x.UserId == UserId).Select(x => x.ClientId).ToList();

            var result = await _context.Clients.Where(x => ClientIDs.Contains(x.Id))
                .Skip(value.skip).Take(value.take)
                .Include(x => x.Claims)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.IdentityProviderRestrictions)
                .AsNoTracking()
                .ToListAsync();

            var total = await _context.Clients.CountAsync();

            var model = new GetResponse()
            {
                clients = result,
                total = total
            };

            return View(model);
        }

        // GET: Client/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || !Exists(id))
            {
                return NotFound();
            }

            var client = await _context.Clients
                .SingleOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Client/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Client/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Client client)
        {
            #region Claims
            var claims = DeserializeFormData<ClientClaim>("Claims");
            if (claims != null && claims.Count > 0)
            {
                client.Claims = claims;
            }
            #endregion

            #region AllowedGrantTypes
            var grants = Request.Form["AllowedGrantTypes"].ToString().Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (grants != null && grants.Length > 0)
            {
                var grantTypes = grants.Select(x => new ClientGrantType() { GrantType = x }).ToList();
                client.AllowedGrantTypes = grantTypes;
            }
            #endregion

            #region AllowedScopes
            var scopes = DeserializeFormData<ClientScope>("AllowedScopes");
            if (scopes != null && scopes.Count > 0)
            {
                client.AllowedScopes = scopes;
            }
            #endregion

            #region ClientSecrets
            var secrets = DeserializeFormData<ClientSecret>("ClientSecrets");
            if (secrets != null && secrets.Count > 0)
            {
                client.ClientSecrets = secrets;
            }
            #endregion

            #region AllowedCorsOrigins
            var corsOrigin = DeserializeFormData<ClientCorsOrigin>("AllowedCorsOrigins");
            if (corsOrigin != null && corsOrigin.Count > 0)
            {
                client.AllowedCorsOrigins = corsOrigin;
            }
            #endregion

            #region RedirectUris
            var redirectUris = DeserializeFormData<ClientRedirectUri>("RedirectUris");
            if (redirectUris != null && redirectUris.Count > 0)
            {
                client.RedirectUris = redirectUris;
            }
            #endregion

            #region PostLogoutRedirectUris
            var logoutRedirectUris = DeserializeFormData<ClientPostLogoutRedirectUri>("PostLogoutRedirectUris");
            if (logoutRedirectUris != null && logoutRedirectUris.Count > 0)
            {
                client.PostLogoutRedirectUris = logoutRedirectUris;
            }
            #endregion

            #region IdentityProviderRestrictions
            var IdPRestrictions = DeserializeFormData<ClientIdPRestriction>("IdentityProviderRestrictions");
            if (IdPRestrictions != null && IdPRestrictions.Count > 0)
            {
                client.IdentityProviderRestrictions = IdPRestrictions;
            }
            #endregion

            if (ModelState.IsValid)
            {
                _context.Add(client);
                await _context.SaveChangesAsync();

                _userContext.UserClients.Add(new AspNetUserClient()
                {
                    ClientId = client.Id,
                    UserId = UserId
                });

                await _userContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(client);
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !Exists(id))
            {
                return NotFound();
            }

            var client = await _context.Clients
                .Include(x => x.Claims)
                .Include(x => x.AllowedGrantTypes)
                .Include(x => x.AllowedScopes)
                .Include(x => x.ClientSecrets)
                .Include(x => x.AllowedCorsOrigins)
                .Include(x => x.RedirectUris)
                .Include(x => x.PostLogoutRedirectUris)
                .Include(x => x.IdentityProviderRestrictions)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Client/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Client client)
        {
            if (!Exists(id))
            {
                return NotFound();
            }

            var src = await _context.Clients
              .Include(x => x.Claims)
              .Include(x => x.AllowedGrantTypes)
              .Include(x => x.AllowedScopes)
              .Include(x => x.ClientSecrets)
              .Include(x => x.AllowedCorsOrigins)
              .Include(x => x.RedirectUris)
              .Include(x => x.PostLogoutRedirectUris)
              .Include(x => x.IdentityProviderRestrictions)
              .AsNoTracking()
              .SingleOrDefaultAsync(m => m.Id == id);

            #region Claims
            List<ClientClaim>
                Claims_Add = null,
                Claims_Update = null,
                Claims_Delete = null;
            var claims = DeserializeFormData<ClientClaim>("Claims");
            if (claims != null && claims.Count > 0)
            {
                Claims_Add = claims.Where(x => x.Id == 0).ToList();
                Claims_Update = claims.Where(x => x.Id > 0).ToList();
                var Ids = claims.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    Claims_Delete = src.Claims.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region AllowedGrantTypes
            List<ClientGrantType>
                GrantTypes_Add = null,
                GrantTypes_Update = null,
                GrantTypes_Delete = null;
            var grants = Request.Form["AllowedGrantTypes"].ToString().Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
            if (grants != null && grants.Length > 0)
            {
                var grantTypes = grants.Select(x => new ClientGrantType() { GrantType = x }).ToList();
                grantTypes.ForEach(x =>
                {
                    var existsGrant = src.AllowedGrantTypes.Where(q => q.GrantType.Equals(x.GrantType)).FirstOrDefault();
                    if (existsGrant != null) { x.Id = existsGrant.Id; }
                });
                GrantTypes_Add = grantTypes.Where(x => x.Id == 0).ToList();
                GrantTypes_Update = grantTypes.Where(x => x.Id > 0).ToList();
                var Ids = grantTypes.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    GrantTypes_Delete = src.AllowedGrantTypes.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region AllowedScopes
            List<ClientScope>
                Scopes_Add = null,
                Scopes_Update = null,
                Scopes_Delete = null;
            var scopes = DeserializeFormData<ClientScope>("AllowedScopes");
            if (scopes != null && scopes.Count > 0)
            {
                Scopes_Add = scopes.Where(x => x.Id == 0).ToList();
                Scopes_Update = scopes.Where(x => x.Id > 0).ToList();
                var Ids = scopes.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    Scopes_Delete = src.AllowedScopes.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region ClientSecrets
            List<ClientSecret>
                Secrets_Add = null,
                Secrets_Update = null,
                Secrets_Delete = null;
            var secrets = DeserializeFormData<ClientSecret>("ClientSecrets");
            if (secrets != null && secrets.Count > 0)
            {
                Secrets_Add = secrets.Where(x => x.Id == 0).ToList();
                Secrets_Update = secrets.Where(x => x.Id > 0).ToList();
                var Ids = secrets.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    Secrets_Delete = src.ClientSecrets.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region AllowedCorsOrigins
            List<ClientCorsOrigin>
                CorsOrigins_Add = null,
                CorsOrigins_Update = null,
                CorsOrigins_Delete = null;
            var corsOrigin = DeserializeFormData<ClientCorsOrigin>("AllowedCorsOrigins");
            if (corsOrigin != null && corsOrigin.Count > 0)
            {
                CorsOrigins_Add = corsOrigin.Where(x => x.Id == 0).ToList();
                CorsOrigins_Update = corsOrigin.Where(x => x.Id > 0).ToList();
                var Ids = corsOrigin.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    CorsOrigins_Delete = src.AllowedCorsOrigins.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region RedirectUris
            List<ClientRedirectUri>
                RedirectUris_Add = null,
                RedirectUris_Update = null,
                RedirectUris_Delete = null;
            var redirectUris = DeserializeFormData<ClientRedirectUri>("RedirectUris");
            if (redirectUris != null && redirectUris.Count > 0)
            {
                RedirectUris_Add = redirectUris.Where(x => x.Id == 0).ToList();
                RedirectUris_Update = redirectUris.Where(x => x.Id > 0).ToList();
                var Ids = redirectUris.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    RedirectUris_Delete = src.RedirectUris.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region PostLogoutRedirectUris
            List<ClientPostLogoutRedirectUri>
                LogoutRedirectUris_Add = null,
                LogoutRedirectUris_Update = null,
                LogoutRedirectUris_Delete = null;
            var logoutRedirectUris = DeserializeFormData<ClientPostLogoutRedirectUri>("PostLogoutRedirectUris");
            if (logoutRedirectUris != null && logoutRedirectUris.Count > 0)
            {
                LogoutRedirectUris_Add = logoutRedirectUris.Where(x => x.Id == 0).ToList();
                LogoutRedirectUris_Update = logoutRedirectUris.Where(x => x.Id > 0).ToList();
                var Ids = logoutRedirectUris.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    LogoutRedirectUris_Delete = src.PostLogoutRedirectUris.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            #endregion

            #region IdentityProviderRestrictions
            List<ClientIdPRestriction>
                IdPRestriction_Add = null,
                IdPRestriction_Update = null,
                IdPRestriction_Delete = null;
            var IdPRestrictions = DeserializeFormData<ClientIdPRestriction>("IdentityProviderRestrictions");
            if (IdPRestrictions != null && IdPRestrictions.Count > 0)
            {
                IdPRestriction_Add = IdPRestrictions.Where(x => x.Id == 0).ToList();
                IdPRestriction_Update = IdPRestrictions.Where(x => x.Id > 0).ToList();
                var Ids = IdPRestrictions.Select(x => x.Id).ToList();
                if (Ids.Count > 0)
                {
                    IdPRestriction_Delete = src.IdentityProviderRestrictions.Where(x => !Ids.Contains(x.Id)).ToList();
                }
            }
            else
            {
                IdPRestriction_Delete = src.IdentityProviderRestrictions;
            }
            #endregion

            #region Submit to database
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(client);
                    await _context.SaveChangesAsync();

                    #region Claims
                    if (Claims_Add != null && Claims_Add.Count > 0)
                    {
                        Claims_Add.ForEach(x => x.Client = client);
                        _context.AddRange(Claims_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Claims_Update != null && Claims_Update.Count > 0)
                    {
                        Claims_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(Claims_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Claims_Delete != null && Claims_Delete.Count > 0)
                    {
                        Claims_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(Claims_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region AllowedGrantTypes
                    if (GrantTypes_Add != null && GrantTypes_Add.Count > 0)
                    {
                        GrantTypes_Add.ForEach(x => x.Client = client);
                        _context.AddRange(GrantTypes_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (GrantTypes_Update != null && GrantTypes_Update.Count > 0)
                    {
                        GrantTypes_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(GrantTypes_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (GrantTypes_Delete != null && GrantTypes_Delete.Count > 0)
                    {
                        GrantTypes_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(GrantTypes_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region AllowedScopes
                    if (Scopes_Add != null && Scopes_Add.Count > 0)
                    {
                        Scopes_Add.ForEach(x => x.Client = client);
                        _context.AddRange(Scopes_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Scopes_Update != null && Scopes_Update.Count > 0)
                    {
                        Scopes_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(Scopes_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Scopes_Delete != null && Scopes_Delete.Count > 0)
                    {
                        Scopes_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(Scopes_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region ClientSecrets
                    if (Secrets_Add != null && Secrets_Add.Count > 0)
                    {
                        Secrets_Add.ForEach(x => x.Client = client);
                        _context.AddRange(Secrets_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (Secrets_Update != null && Secrets_Update.Count > 0)
                    {
                        Secrets_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(Secrets_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (Secrets_Delete != null && Secrets_Delete.Count > 0)
                    {
                        Secrets_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(Secrets_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region AllowedCorsOrigins
                    if (CorsOrigins_Add != null && CorsOrigins_Add.Count > 0)
                    {
                        CorsOrigins_Add.ForEach(x => x.Client = client);
                        _context.AddRange(CorsOrigins_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (CorsOrigins_Update != null && CorsOrigins_Update.Count > 0)
                    {
                        CorsOrigins_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(CorsOrigins_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (CorsOrigins_Delete != null && CorsOrigins_Delete.Count > 0)
                    {
                        CorsOrigins_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(CorsOrigins_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region RedirectUris
                    if (RedirectUris_Add != null && RedirectUris_Add.Count > 0)
                    {
                        RedirectUris_Add.ForEach(x => x.Client = client);
                        _context.AddRange(RedirectUris_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (RedirectUris_Update != null && RedirectUris_Update.Count > 0)
                    {
                        RedirectUris_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(RedirectUris_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (RedirectUris_Delete != null && RedirectUris_Delete.Count > 0)
                    {
                        RedirectUris_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(RedirectUris_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region PostLogoutRedirectUris
                    if (LogoutRedirectUris_Add != null && LogoutRedirectUris_Add.Count > 0)
                    {
                        LogoutRedirectUris_Add.ForEach(x => x.Client = client);
                        _context.AddRange(LogoutRedirectUris_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (LogoutRedirectUris_Update != null && LogoutRedirectUris_Update.Count > 0)
                    {
                        LogoutRedirectUris_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(LogoutRedirectUris_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (LogoutRedirectUris_Delete != null && LogoutRedirectUris_Delete.Count > 0)
                    {
                        LogoutRedirectUris_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(LogoutRedirectUris_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion

                    #region IdentityProviderRestrictions
                    if (IdPRestriction_Add != null && IdPRestriction_Add.Count > 0)
                    {
                        IdPRestriction_Add.ForEach(x => x.Client = client);
                        _context.AddRange(IdPRestriction_Add);
                        await _context.SaveChangesAsync();
                    }
                    if (IdPRestriction_Update != null && IdPRestriction_Update.Count > 0)
                    {
                        IdPRestriction_Update.ForEach(x => x.Client = client);
                        _context.UpdateRange(IdPRestriction_Update);
                        await _context.SaveChangesAsync();
                    }
                    if (IdPRestriction_Delete != null && IdPRestriction_Delete.Count > 0)
                    {
                        IdPRestriction_Delete.ForEach(x => x.Client = null);
                        _context.RemoveRange(IdPRestriction_Delete);
                        await _context.SaveChangesAsync();
                    }
                    #endregion
                }

                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Index");
            }
            #endregion

            return View(client);
        }

        // GET: Client/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || !Exists(id))
            {
                return NotFound();
            }

            var entity = await _context.Clients
                .SingleOrDefaultAsync(m => m.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: Client/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var entity = await _context.Clients.SingleOrDefaultAsync(m => m.Id == id);
            if (entity != null)
            {
                _context.Clients.Remove(entity);
                await _context.SaveChangesAsync();
            }

            var userClient = await _userContext.UserClients
              .SingleOrDefaultAsync(x => x.ClientId == id && x.UserId == UserId);
            if (userClient != null)
            {
                _userContext.UserClients.Remove(userClient);
                await _userContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private bool Exists(long? id)
        {
            if (User.IsInRole(AppConstant.Roles.Administrators))
            {
                return _context.Clients.Any(e => e.Id == id);
            }

            if (!_userContext.UserClients.Any(x => x.ClientId == id && x.UserId == UserId))
            {
                return false;
            }

            return _context.Clients.Any(e => e.Id == id);
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
