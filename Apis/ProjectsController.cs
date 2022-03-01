using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using OAuthApp.Filters;
using System.Collections.Generic;

namespace OAuthApp.Apis
{
    [SwaggerTag("项目")]
    public class ProjectsController : BaseController
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "Projects")]
        [EncryptResultFilter]
        public IActionResult List(int skip, int take)
        {
            var _projectIDs = projectIDs();

            var q = _context.Projects
                .Where(x => !x.IsDelete &&
                (x.UserID == UserID || _projectIDs.Contains(x.ID)))
                .AsQueryable();

            var total = q.Count();

            var data = q.Skip(skip).Take(take)
                .OrderByDescending(x => x.ID)
                .ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "Project")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var _projectIDs = projectIDs();

            var result = _context.Projects
                .FirstOrDefault(x => x.ID == id && 
                (x.UserID == UserID || _projectIDs.Contains(x.ID)));

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "ProjectPut")]
        public IActionResult Put(Project project)
        {
            if (UserID != project.UserID)
            {
                return NotFound();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return Error(ex.Message);
            }

            return NoContent();
        }

        [HttpPost]
        [SwaggerOperation(OperationId = "ProjectPost")]
        public IActionResult Post(Project project)
        {
            project.UserID = UserID;

            _context.Projects.Add(project);

            _context.SaveChanges();

            return OK(new { id = project.ID });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "ProjectDelete")]
        public IActionResult Delete(long id)
        {
            var result = _context.Projects
               .FirstOrDefault(x => x.ID == id && x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(result);

            _context.SaveChanges();

            return OK(true);
        }

        List<long> projectIDs()
        {
            return _context.Teams.Where(x =>
           x.ChannelCode == ChannelCodes.Project &&
           x.UserID == UserID)
               .Select(x => long.Parse(x.ChannelAppID)).ToList();
        }
    }
}
