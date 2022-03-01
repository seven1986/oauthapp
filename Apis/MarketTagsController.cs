using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using System;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用市场标签")]
    public class MarketTagsController : BaseController
    {
        private readonly AppDbContext _context;

        public MarketTagsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(OperationId = "MarketTags")]
        [EncryptResultFilter]
        public IActionResult List(string channelCode, int skip, int take)
        {
            var q = _context.MarketTags
                .Where(x => !x.IsDelete && x.ShowFlag && x.ChannelCode == channelCode)
                .AsQueryable();

            var total = q.Count();

            var data = q.Skip(skip).Take(take)
                .OrderByDescending(x => x.ID)
                .OrderByDescending(x => x.Priority).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "MarketTag")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var marketTag = _context.MarketTags.Find(id);

            if (marketTag == null)
            {
                return NotFound();
            }

            return OK(marketTag);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "MarketTagPut")]
        public IActionResult Put(long id, MarketTag marketTag)
        {
            if (id != marketTag.ID)
            {
                return NotFound();
            }

            _context.Entry(marketTag).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "MarketTagPost")]
        public IActionResult Post(MarketTag marketTag)
        {
            marketTag.UserID = UserID;

            _context.MarketTags.Add(marketTag);

            _context.SaveChanges();

            return OK(new { id = marketTag.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "MarketTagDelete")]
        public IActionResult Delete(long id)
        {
            var marketTag = _context.MarketTags.Find(id);

            if (marketTag == null)
            {
                return NotFound();
            }

            _context.MarketTags.Remove(marketTag);

            _context.SaveChanges();

            return OK(true);
        }
    }
}
