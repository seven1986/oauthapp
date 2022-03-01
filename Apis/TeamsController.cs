using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.ApiModels.TeamsController;
using System;
using OAuthApp.Filters;
using System.Linq;

namespace OAuthApp.Apis
{
    [SwaggerTag("团队")]
    public class TeamsController : BaseController
    {
        private readonly AppDbContext _context;

        public TeamsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "Teams")]
        [EncryptResultFilter]
        public IActionResult List(string channelCode,string channelAppId)
        {
           var result = _context.Query<ListResponseItem>(@"SELECT A.ID, A.UserID,A.Role,A.Permission," +
                @" A.LastUpdate,B.Avatar,B.Email,B.NickName,B.Phone,B.UserName" +
                @" FROM Teams A JOIN Users B ON A.UserID = B.ID" +
                @" WHERE A.ChannelCode = @ChannelCode AND A.ChannelAppID=@ChannelAppID", new {
                    ChannelCode = channelCode,
                    ChannelAppID = channelAppId
                });

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "TeamPut")]
        [EncryptResultFilter]
        public IActionResult Put(long id, Team team)
        {
            if (id != team.ID)
            {
                return BadRequest();
            }

            _context.Entry(team).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "TeamPost")]
        public IActionResult Post(Team team)
        {
            if (_context.Teams.Any(x => x.ChannelCode == team.ChannelCode &&
             x.ChannelAppID == team.ChannelAppID &&
             x.UserID == team.UserID))
            {
                return OK(true);
            }

            _context.Teams.Add(team);
            _context.SaveChanges();

            return Ok(true);
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "TeamDelete")]
        public IActionResult Delete(long id)
        {
            var team = _context.Teams.Find(id);

            if (team == null)
            {
                return NotFound();
            }

            _context.Teams.Remove(team);

            _context.SaveChanges();

            return Ok(true);
        }
    }
}
