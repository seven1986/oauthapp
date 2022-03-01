using OAuthApp.Tenant;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;

namespace OAuthApp.Apis
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        protected int UserID
        {
            get
            {
                var userIdClaim = GetUserClaim(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrWhiteSpace(userIdClaim))
                {
                    var userId = int.Parse(userIdClaim);

                    return userId;
                }

                return 0;
            }
        }

        protected int UnionID
        {
            get
            {
                var userIdClaim = GetUserClaim(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrWhiteSpace(userIdClaim))
                {
                    var userId = int.Parse(userIdClaim);

                    return userId;
                }

                return 0;
            }
        }

        protected string ClientID
        {
            get
            {
                return GetUserClaim(TenantClaimTypes.ClientId);
            }
        }

        private long _AppID = 0;

        protected long AppID
        {
            get
            {
                if (_AppID == 0)
                {
                    if (Request.RouteValues.ContainsKey("appId"))
                    {
                        long.TryParse(
                            Request.RouteValues["appId"].ToString(),
                            out _AppID);
                    }
                }

                return _AppID;
            }
        }

        protected string GetUserClaim(string claimType)
        {
            var claims = HttpContext.User.Identity as ClaimsIdentity;

            var claimItem = claims.FindFirst(claimType);

            if (claimItem != null)
            {
                return claimItem.Value;
            }

            return string.Empty;
        }

        protected JsonResult OK(object data)
        {
            return Json(200, data, string.Empty);
        }

        protected JsonResult Error(string err, int code = 500)
        {
            return Json(code, string.Empty, err);
        }

        protected JsonResult Json(int code, object data, string err)
        {
            return new JsonResult(new { code, data, err });
        }

        public static List<string> ExecuteCMD(params string[] cmds)
        {
            var result = new List<string>();

            using (var p = new Process())
            {
                var info = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                p.StartInfo = info;

                p.OutputDataReceived += (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        result.Add(e.Data);
                    }
                };

                p.ErrorDataReceived += (s, e) =>
                {
                    //Console.WriteLine(e.Data);
                };

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                using (var sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        for(var i=0;i<cmds.Length;i++)
                        {
                            sw.WriteLine(cmds[i]);
                        }

                        //sw.WriteLine("cd /d D:\\jixiu_git\\campaign\\ossutil64");
                        //sw.WriteLine("ossutil64 du oss://webplus-cn-shanghai-s-619b5561f968dd14ce2dec06/serverPath --block-size MB");
                        //sw.WriteLine("cd  ..");
                        //sw.WriteLine("rmdir " + packageExtractPath + " /s");
                        //sw.WriteLine("Y");
                        //sw.WriteLine("rm " + packageName + " -f");
                    }
                }

                p.WaitForExit();
            }

            return result;
        }
    }
}
