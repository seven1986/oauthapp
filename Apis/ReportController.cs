using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using OAuthApp.Data;
using System.Linq;
using OAuthApp.ApiModels.ReportController;
using OAuthApp.Filters;

namespace OAuthApp.Apis
{
    [SwaggerTag("报表")]
    public class ReportController : BaseController
    {
        private readonly AppDbContext _appContext;

        public ReportController(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        [HttpGet("{id}/Users")]
        [SwaggerOperation(OperationId = "ReportUsers")]
        [EncryptResultFilter]
        public IActionResult Users(long id)
        {
            var result = _appContext.AppUsers
                .Where(x => x.AppID == id).CountAsync().Result;

            return OK(result);
        }

        [HttpGet("{id}/UserByDays")]
        [SwaggerOperation(OperationId = "ReportUserByDays")]
        [EncryptResultFilter]
        public IActionResult UserByDays(long id,int take)
        {
            var result = _appContext.Query<AppUsers30DaysResponse>(@"SELECT LastUpdate, COUNT(1) Total " +
                " FROM AppUsers WHERE AppID = " + id + " GROUP BY date(LastUpdate) ORDER BY LastUpdate DESC" +
                " LIMIT " + take);

            return OK(result);
        }
    }
}
