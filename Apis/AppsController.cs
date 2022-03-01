using System;
using OAuthApp.Filters;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAuthApp.Data;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using FluentFTP;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authorization;
using OAuthApp.Tenant;
using OAuthApp.Services;
using System.Collections.Generic;
using OAuthApp.ApiModels.AppsController;

namespace OAuthApp.Apis
{
    [SwaggerTag("应用")]
    public class AppsController : BaseController
    {
        private readonly IWebHostEnvironment _env;
        private readonly AppDbContext _context;
        private readonly TenantDbContext _tenantDbContext;
        private readonly TenantContext _tenant;
        private readonly UploadService _uploader;

        public AppsController(AppDbContext context,
            TenantDbContext tenantDbContext,
            IWebHostEnvironment env,
            IHttpContextAccessor contextAccessor,
            UploadService uploader)
        {
            _context = context;
            _tenantDbContext = tenantDbContext;
            _env = env;
            _tenant = contextAccessor.HttpContext.GetTenantContext();
            _uploader = uploader;
        }

        [HttpGet("Market")]
        [SwaggerOperation(OperationId = "AppMarket")]
        [EncryptResultFilter]
        [AllowAnonymous]
        public IActionResult Market(string tag,int skip, int take)
        {
            var q = _context.Apps.Where(x => x.Share && !x.IsDelete).AsQueryable();

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
        [SwaggerOperation(OperationId = "Apps")]
        [EncryptResultFilter]
        public IActionResult List(long projectId, int skip, int take)
        {
            var _teamProjects = projectIDs();

            var q = _context.Apps
                .Where(x => !x.IsDelete &&
                (x.ProjectID == projectId && x.UserID == UserID)
                || _teamProjects.Contains(x.ProjectID)).AsQueryable();

            var total = q.Count();

            var data = q.Skip(skip).Take(take).OrderByDescending(x => x.ID).ToList();

            return OK(new
            {
                total,
                data
            });
        }

        [HttpGet("{id}")]
        [SwaggerOperation(OperationId = "App")]
        [EncryptResultFilter]
        public IActionResult Get(long id)
        {
            var _teamProjects = projectIDs();

            var result = _context.Apps
               .FirstOrDefault(x => x.ID == id && 
               (x.UserID == UserID || _teamProjects.Contains(x.ProjectID)));

            if (result == null)
            {
                return NotFound();
            }

            return OK(result);
        }

        [HttpPut("{id}")]
        [SwaggerOperation(OperationId = "AppPut")]
        public IActionResult Put(long id, App app)
        {
            var _teamProjects = projectIDs();

            if (id != app.ID || !_context.Apps.Any(x => x.ID == id && 
            (x.UserID == UserID || _teamProjects.Contains(x.ProjectID))))
            {
                return NotFound();
            }

            if (_context.Apps.Any(x => x.ID != id && x.ServerPath.Equals(app.ServerPath)))
            {
                return Error("已存在的站点路径");
            }

            app.UserID = UserID;

            _context.Entry(app).State = EntityState.Modified;

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
        [SwaggerOperation(OperationId = "AppPost")]
        public IActionResult Post(App app)
        {
            app.UserID = UserID;

            _context.Apps.Add(app);

            _context.SaveChanges();

            return OK(new { id = app.ID, serverPath = app.ServerPath });
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(OperationId = "AppDelete")]
        public IActionResult Delete(long id)
        {
            var _teamProjects = projectIDs();

            var result = _context.Apps
                .FirstOrDefault(x => x.ID == id &&
                (x.UserID == UserID || _teamProjects.Contains(x.ProjectID)));

            if (result == null)
            {
                return NotFound();
            }

            _context.Execute("DELETE FROM AppChatMessages WHERE AppID = " + id);
            _context.Execute("DELETE FROM AppRanks WHERE AppID = " + id);
            _context.Execute("DELETE FROM AppUsers WHERE AppID = " + id);
            _context.Execute("DELETE FROM AppVersions WHERE AppID = " + id);

            var HasStorageTable = _context.QueryFirstOrDefault<int>
                ($"SELECT count(1) AS Total FROM sqlite_master WHERE type=\"table\" AND name = \"_AppStorage_${id}\"");

            if (HasStorageTable > 0)
            {
                _context.Execute("DROP TABLE _AppStorage_" + id);
            }

            _context.Apps.Remove(result);
            _context.SaveChanges();

            return OK(true);
        }

        [HttpGet("{id}/Transfer")]
        [SwaggerOperation(OperationId = "AppTransfer")]
        [EncryptResultFilter]
        public IActionResult Transfer(long id,long projectId)
        {
            var result = _context.Apps.FirstOrDefault(x => x.ID == id &&x.UserID == UserID);

            if (result == null)
            {
                return NotFound();
            }

            result.ProjectID = projectId;

            _context.SaveChanges();

            return OK(result);
        }

        [HttpPost("Release")]
        [SwaggerOperation(OperationId = "AppRelease")]
        public async Task<IActionResult> Release(
            [Required][FromQuery] long id,
            [Required][FromQuery] long serverID,
            [Required][FromQuery] bool rollback,
            [FromForm] IFormFile file)
        {
            var app = _context.Apps.Where(x => x.ID.Equals(id) && x.IsDelete == false)
                .FirstOrDefault();

            if (app == null)
            {
                return Error("应用不存在");
            }

            if (string.IsNullOrWhiteSpace(app.ServerPath))
            {
                return Error("未设置服务器目录");
            }

            var server = _tenantDbContext.TenantServers.FirstOrDefault(x => x.ID.Equals(serverID));

            if (server == null)
            {
                return Error("发布服务器不存在");
            }

            if (file.Length < 0 || file.Length > 10 * 1024 * 1024)
            {
                return Error("程序应小于10MB");
            }

            var fileName = Guid.NewGuid().ToString("n");

            var destinationPath = Path.Combine(_env.WebRootPath,
                "_temp",
                id.ToString(),
                DateTime.Now.ToString("yyyyMMdd"),
                fileName);

            byte[] fileData = null;

            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms, CancellationToken.None);
                fileData = ms.ToArray();
            }

            using (var fs = file.OpenReadStream())
            {
                using var zip = new ZipArchive(fs, ZipArchiveMode.Read, true);

                zip.ExtractToDirectory(destinationPath);
            }

            #region ftp发布
            using (var client = new FtpClient(server.ServerUrl))
            {
                client.Credentials = new NetworkCredential(server.UserName, server.Password);

                client.Connect();

                client.UploadDirectory(destinationPath,
                    string.Format(Path.Combine(server.RootFolder, app.ServerPath), _tenant.Id),
                    FtpFolderSyncMode.Update,
                    FtpRemoteExists.Overwrite);

                client.RetryAttempts = 3;

                client.Disconnect();
            }
            #endregion

            if (!rollback)
            {
                var version = DateTime.Now.ToString("yyyyMMddHHmmss");

                var savePath = $"{_tenant.Id}/{ChannelCodes.AppVersion}/{id}/{version}_{file.FileName}";

                _uploader.Upload(savePath, file);

                _context.AppVersions.Add(new AppVersion()
                {
                    AppID = id,
                    AppServerID = server.ID,
                    PackageBackupUri = AppConst.BlobServer + "/" + savePath,
                    Ver = version,
                    UserID = UserID
                });

                _context.SaveChanges();

                #region 累计版本备份用量
                _tenantDbContext.TenantOrders.Add(new TenantOrder()
                {
                    Amount = fileData.Length,
                    ChannelAppID = id.ToString(),
                    ChannelCode = ChannelCodes.AppVersion,
                    TenantID = _tenant.Id
                });
                _tenantDbContext.SaveChanges();
                #endregion
            }

            #region 累计站点空间用量
            _tenantDbContext.TenantOrders.Add(new TenantOrder()
            {
                Amount = fileData.Length,
                ChannelAppID = id.ToString(),
                ChannelCode = ChannelCodes.App,
                TenantID = _tenant.Id
            });
            _tenantDbContext.SaveChanges();
            #endregion

            return OK(server.WebSiteUrl + "/" + _tenant.Id + "/" + app.ServerPath);
        }

        List<long> projectIDs()
        {
            return _context.Teams.Where(x =>
           x.ChannelCode == ChannelCodes.Project &&
           x.UserID == UserID)
               .Select(x => long.Parse(x.ChannelAppID)).ToList();
        }

        // JS 客户端
        [HttpGet("{id}/Info")]
        [SwaggerOperation(OperationId = "AppInfo")]
        [AllowAnonymous]
        public IActionResult Info(long id)
        {
            var info = _context.Apps.Find(id);

            var props = _context.PropertySettings
                .Where(x => x.ChannelCode == ChannelCodes.App &&
                x.ChannelAppId == id &&
                x.Tag == PropertyTag.Client)
                .Select(x => new { code = x.Name, value = x.Value })
                .ToList();

            var blobs = _tenantDbContext.TenantBlobs
               .Where(x => x.ChannelCode == ChannelCodes.App &&
               x.ChannelAppID == id.ToString())
               .Select(x => new { code = x.Name, value = x.Value })
               .ToList();

            return OK(new
            {
                info,
                props,
                blobs
            });
        }

        /// <summary>
        /// 服务器文件列表
        /// </summary>
        [HttpGet("{id}/Files")]
        [SwaggerOperation(OperationId = "AppFiles")]
        [EncryptResultFilter]
        public IActionResult AppFiles(long id)
        {
            var server = _tenantDbContext.TenantServers
                .Where(x => x.Tag.Equals("site,public")).FirstOrDefault();

            if (server == null)
            {
                return Error("不存在的服务器");
            }

            var appPath = _context.Apps.Where(x => x.ID == id)
                .Select(x => x.ServerPath).FirstOrDefault();

            if(string.IsNullOrWhiteSpace(appPath))
            {
                return Error("不存在的应用");
            }

            var result = GetList(server.ServerUrl, server.UserName,
                server.Password,
                 string.Format(server.RootFolder,_tenant.Id),
                appPath);

            return OK(result);
        }

        private AppFileItem GetList(string ftp_url, string ftp_userName, string ftp_password, string rootPath, string subPath)
        {
            var startPath = $"{rootPath}{subPath}";

            var root = new AppFileItem(startPath.GetHashCode(), subPath, startPath, "directory")
            {
                children = new List<AppFileItem>()
            };

            using (var conn = new FtpClient(ftp_url, ftp_userName, ftp_password))
            {
                conn.Connect();

                foreach (var item in conn.GetListing(startPath))
                {
                    if (item.Type == FtpFileSystemObjectType.Directory)
                    {
                        var directoryItem = new AppFileItem(item.GetHashCode(), item.Name, item.FullName, "directory")
                        {
                            children = new List<AppFileItem>()
                        };

                        root.children.Add(directoryItem);

                        _GetList(conn, item.FullName, directoryItem);
                    }

                    else if (item.Type == FtpFileSystemObjectType.File)
                    {
                        root.children.Add(new AppFileItem(item.GetHashCode(), item.Name, item.FullName, Path.GetExtension(item.Name))
                        {
                            children = new List<AppFileItem>()
                        });
                    }
                }
            }

            return root;
        }

        private void _GetList(FtpClient conn, string path, AppFileItem node)
        {
            foreach (var item in conn.GetListing(path))
            {
                if (item.Type == FtpFileSystemObjectType.Directory)
                {
                    var directoryItem = new AppFileItem(item.GetHashCode(), item.Name, item.FullName, "directory")
                    {
                        children = new List<AppFileItem>()
                    };

                    node.children.Add(directoryItem);

                    _GetList(conn, item.FullName, directoryItem);
                }

                else if (item.Type == FtpFileSystemObjectType.File)
                {
                    node.children.Add(new AppFileItem(item.GetHashCode(), item.Name, item.FullName, Path.GetExtension(item.Name))
                    {
                        children = new List<AppFileItem>()
                    });
                }
            }
        }
    }
}
