using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using Microsoft.AspNetCore.Authorization;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("API")]
    public class ApisController : BaseController
    {
        private readonly ApiDbContext _context;

        public ApisController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet("Market")]
        [SwaggerOperation(OperationId = "ApiMarket")]
        [AllowAnonymous]
        [EncryptResultFilter]
        public IActionResult Market(string tag, int skip, int take)
        {
            var q = _context.Apis.AsQueryable();

            q = q.Where(x => x.Show && !x.IsDelete);

            if (!string.IsNullOrWhiteSpace(tag))
            {
                q = q.Where(x => x.Tags.Contains(tag));
            }

            var total = q.Count();

            var data = q.Skip(skip).Take(take)
                .Select(x => new
                {
                    ID = x.ID,
                    Name = x.Name,
                    ApiKey = x.ApiKey,
                    LogoUri = x.LogoUri,
                    summary = x.Summary
                }).OrderByDescending(x => x.ID).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "Apis")]
        [EncryptResultFilter]
        public IActionResult List(int skip, int take)
        {
            var q = _context.Apis
                .Where(x => !x.IsDelete && x.UserID == UserID).AsQueryable();

            var total = q.Count();

            var data = q.Skip(skip).Take(take).OrderByDescending(x => x.ID).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{apiKey}")]
        [SwaggerOperation(OperationId = "Api")]
        [AllowAnonymous]
        [EncryptResultFilter]
        public IActionResult Get(string apiKey)
        {
            var result = _context.Apis
                .FirstOrDefault(x => x.ApiKey == apiKey && !x.IsDelete);

            if (result == null)
            {
                return NotFound();
            }

            var codeGens = _context.Query<CodeGenerator>(@"SELECT A.* FROM CodeGenerators A" +
              " JOIN ApiCodeGenerators B ON B.CodeGeneratorID = A.ID" +
              " WHERE B.ApiID = " + result.ID);

            return OK(new
            {
                api = result,
                codegens = codeGens
            });
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "ApiPut")]
        public IActionResult Put(long id, Api api)
        {
            api.UserID = UserID;

            _context.Entry(api).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "ApiPost")]
        public IActionResult Post(Api api)
        {
            api.UserID = UserID;

            _context.Apis.Add(api);

            _context.SaveChanges();

            return OK(new { id = api.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "ApiDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.Apis
                .FirstOrDefault(x => x.ID == id && x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            _context.Execute("DELETE FROM ApiCodeGenerators WHERE ApiID = " + id);
            _context.Execute("DELETE FROM ApiSubscribers WHERE ApiID = " + id);

            _context.Apis.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
