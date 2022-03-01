using System.Linq;
using Microsoft.AspNetCore.Mvc;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("API CodeGen")]
    public class ApiCodeGeneratorsController : BaseController
    {
        private readonly ApiDbContext _context;

        public ApiCodeGeneratorsController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "ApiCodeGens")]
        [EncryptResultFilter]
        public IActionResult List(long apiId, int skip, int take)
        {
            var q = _context.ApiCodeGenerators.Where(x => x.ApiID == apiId).AsQueryable();

            var total = q.Count();

            var data = q.Skip(skip).Take(take).OrderByDescending(x => x.ID).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "ApiCodeGen")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var result = _context.ApiCodeGenerators
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "ApiCodeGenPost")]
        public IActionResult Post(ApiCodeGenerator apiCodeGenerator)
        {
            if(_context.ApiCodeGenerators.Any(x=>x.ApiID==apiCodeGenerator.ApiID
            &&x.CodeGeneratorID==apiCodeGenerator.CodeGeneratorID))
            {
                return Error("请勿重复添加");
            }

            _context.ApiCodeGenerators.Add(apiCodeGenerator);

            _context.SaveChanges();

            return OK(new { id = apiCodeGenerator.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "ApiCodeGenDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.ApiCodeGenerators
                .FirstOrDefault(x => x.ID == id);

            if (result == null)
            {
                return NotFound();
            }

            _context.ApiCodeGenerators.Remove(result);
            _context.SaveChanges();

            return OK(true);
        }
    }
}
