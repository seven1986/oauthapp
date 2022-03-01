using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用聊天室")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AppChatMessagesController : BaseController
    {
        private readonly AppDbContext _context;

        public AppChatMessagesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/AppChatMessages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppChatMessage>>> GetAppChatMessages(long appId)
        {
            return await _context.AppChatMessages.Where(x => x.AppID == appId).ToListAsync();
        }

        // GET: api/AppChatMessages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppChatMessage>> GetAppChatMessage(long id)
        {
            var appChatMessage = await _context.AppChatMessages.FindAsync(id);

            if (appChatMessage == null)
            {
                return NotFound();
            }

            return appChatMessage;
        }

        // PUT: api/AppChatMessages/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppChatMessage(AppChatMessage appChatMessage)
        {
            _context.Entry(appChatMessage).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/AppChatMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AppChatMessage>> PostAppChatMessage(AppChatMessage appChatMessage)
        {
            _context.AppChatMessages.Add(appChatMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppChatMessage", new { id = appChatMessage.ID }, appChatMessage);
        }

        // DELETE: api/AppChatMessages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppChatMessage(long id)
        {
            var appChatMessage = await _context.AppChatMessages.FindAsync(id);
            if (appChatMessage == null)
            {
                return NotFound();
            }

            _context.AppChatMessages.Remove(appChatMessage);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
