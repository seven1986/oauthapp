using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("CodeGen")]
    public class CodeGeneratorsController : BaseController
    {
        private readonly ApiDbContext _context;

        public CodeGeneratorsController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet("Market")]
        [SwaggerOperation(OperationId = "CodeGenMarket")]
        [EncryptResultFilter]
        public IActionResult Market(string tag, int skip, int take)
        {
            var q = _context.CodeGenerators.
                Where(x => x.Show && !x.IsDelete).AsQueryable();

            if (!string.IsNullOrWhiteSpace(tag))
            {
                q = q.Where(x => x.Tags.Contains(tag));
            }

            var total = q.Count();

            var data = q.Skip(skip).Take(take).OrderByDescending(x => x.ID).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "CodeGens")]
        [EncryptResultFilter]
        public IActionResult List(int skip, int take)
        {
            var q = _context.CodeGenerators
               .Where(x => !x.IsDelete && x.UserID == UserID)
               .AsQueryable();

            var total = q.Count();

            var data = q.Skip(skip).Take(take).OrderByDescending(x => x.ID).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "CodeGen")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.CodeGenerators
               .FirstOrDefault(x => x.ID == id && !x.IsDelete);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }
        
        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "CodeGenPut")]
        [EncryptResultFilter]
        public IActionResult Put(long id, CodeGenerator codeGenerator)
        {
            codeGenerator.UserID = UserID;

            _context.Entry(codeGenerator).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "CodeGenPost")]
        public IActionResult Post(CodeGenerator codeGenerator)
        {
            codeGenerator.UserID = UserID;

            _context.CodeGenerators.Add(codeGenerator);

            _context.SaveChanges();

            return OK(new { id = codeGenerator.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "CodeGenDelete")]
        public IActionResult Delete(long id)
        {
            var codeGenerator = _context.CodeGenerators
                .FirstOrDefault(x => x.ID == id && x.UserID == UserID);

            if (codeGenerator == null)
            {
                return NotFound();
            }

            _context.CodeGenerators.Remove(codeGenerator);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
