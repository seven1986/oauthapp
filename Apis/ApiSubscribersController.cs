using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace OAuthApp.Apis
{
    [SwaggerTag("API订阅")]
    public class ApiSubscribersController : BaseController
    {
        private readonly ApiDbContext _context;

        public ApiSubscribersController(ApiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "ApiSubscribers")]
        public async Task<ActionResult<IEnumerable<ApiSubscriber>>> GetApiSubscribers()
        {
            return await _context.ApiSubscribers.ToListAsync();
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "ApiSubscriber")]
        public async Task<ActionResult<ApiSubscriber>> GetApiSubscriber(long id)
        {
            var apiSubscriber = await _context.ApiSubscribers.FindAsync(id);

            if (apiSubscriber == null)
            {
                return NotFound();
            }

            return apiSubscriber;
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "ApiSubscriberPut")]
        public async Task<IActionResult> PutApiSubscriber(long id, ApiSubscriber apiSubscriber)
        {
            if (id != apiSubscriber.ID)
            {
                return BadRequest();
            }

            _context.Entry(apiSubscriber).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ApiSubscriberExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "ApiSubscriberPost")]
        public async Task<ActionResult<ApiSubscriber>> PostApiSubscriber(ApiSubscriber apiSubscriber)
        {
            _context.ApiSubscribers.Add(apiSubscriber);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetApiSubscriber", new { id = apiSubscriber.ID }, apiSubscriber);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "ApiSubscriberDelete")]
        public async Task<IActionResult> DeleteApiSubscriber(long id)
        {
            var apiSubscriber = await _context.ApiSubscribers.FindAsync(id);
            if (apiSubscriber == null)
            {
                return NotFound();
            }

            _context.ApiSubscribers.Remove(apiSubscriber);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ApiSubscriberExists(long id)
        {
            return _context.ApiSubscribers.Any(e => e.ID == id);
        }
    }
}
