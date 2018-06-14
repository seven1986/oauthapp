using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.MicroService.AzureJobs.Models;
using IdentityServer4.MicroService.AzureJobs.Services;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IdentityServer4.MicroService.AzureJobs
{
    public class Fns
    {
        readonly static AzureStorageService storage = new AzureStorageService();

        readonly static EmailService mail = new EmailService();

        /// <summary>
        /// 发布NPM包
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ReleasePackage_NPM(string packageUrl, TextWriter log)
        {
            log.WriteLine(packageUrl);

            using (var client = new WebClient())
            {
                // 解压文件夹名称
                var packageDirectoryName = "npmpackage";

                // 压缩包名
                var packageName = Guid.NewGuid().ToString("N") + ".zip";

                // zip包下载文件夹路径
                var packageExtractToDirectory = AppDomain.CurrentDomain.BaseDirectory + packageDirectoryName + @"\";

                if (!Directory.Exists(packageExtractToDirectory))
                {
                    Directory.CreateDirectory(packageExtractToDirectory);
                }

                // zip包下载路径
                var packagePath = packageExtractToDirectory + packageName;

                await client.DownloadFileTaskAsync(new Uri(packageUrl), packagePath);

                ZipFile.ExtractToDirectory(packagePath, packageExtractToDirectory);

                File.Delete(packagePath);

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
                    p.OutputDataReceived += (s, e) => log.WriteLine(e.Data);
                    p.ErrorDataReceived += (s, e) => log.WriteLine(e.Data);

                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();

                    using (var sw = p.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            sw.WriteLine("cd  " + packageExtractToDirectory);
                            sw.WriteLine("npm publish");
                            sw.WriteLine("cd  ..");
                            sw.WriteLine("rmdir " + packageDirectoryName + " /s");
                            sw.WriteLine("Y");
                            sw.WriteLine("rm " + packageName + " -f");
                        }
                    }

                    p.WaitForExit();
                }
            }
        }

        /// <summary>
        /// 发送邮件通知给订阅者
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ApiresourcePublishNotify(string apiId, TextWriter log)
        {
            log.WriteLine(apiId);

            var tb = await storage.CreateTableAsync("ApiResourceSubscriptions");

            var query = new TableQuery<ApiResourceSubscriptionEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, apiId));

            var result = await storage.ExecuteQueryAsync(tb, query);

            var mails = result.Select(x => x.RowKey).ToList();

            if (mails.Count < 1)
            {
                log.WriteLine("没有订阅者");

                return;
            }

            var vars = new Dictionary<string, string[]>();
            vars.Add("%apiId%", new string[mails.Count]);
            for (var i = 0; i < mails.Count; i++)
            {
                vars["%apiId%"][i] = apiId;
            }

            log.WriteLine(string.Join(",", mails));

            try
            {
                var sendResult = await mail.SendEmail(SendCloudMailTemplates.apiresource_published_notify,
                    mails.ToArray(),
                    vars);
            }
            catch (Exception ex)
            {
                log.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 同步Github标签
        /// </summary>
        /// <param name="value"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ApiresourcePublishGithub(GithubQueueModel value, TextWriter log)
        {
            log.WriteLine(JsonConvert.SerializeObject(value));

            if (string.IsNullOrWhiteSpace(value.token)) { return; }

            var client = new GithubService(value.userAgent, value.owner, value.repo, value.token);

            var exists = await client.ReposCreateIfNotExistsAsync(value.repo);

            if (!exists.IsSuccessStatusCode)
            {
                var ReasonPhrase = exists.ReasonPhrase;

                var ResponseMessage = await exists.Content.ReadAsStringAsync();

                log.WriteLine(ReasonPhrase + ":" + ResponseMessage);
            }

            var labels = await client.LabelsAsync();

            var operations = await client.OperationsAsync(value.swaggerUrl);

            foreach (var o in operations)
            {
                if (labels.Any(x => x.name.ToLower().Equals(o.Key.ToLower())))
                {
                    continue;
                }

                var result = await client.LabelPostAsync(o.Key, o.Value);

                if (!result.IsSuccessStatusCode)
                {
                    var ReasonPhrase = result.ReasonPhrase;

                    var RequestMessage = result.Content.ReadAsStringAsync().Result;
                }
            }
        }

        /// <summary>
        /// 初始化Github项目中readthedocs的配置
        /// </summary>
        /// <param name="value"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ApiresourcePublishGithubReadthedocs(GithubQueueModel value, TextWriter log)
        {
            log.WriteLine(JsonConvert.SerializeObject(value));

            if (string.IsNullOrWhiteSpace(value.token)) { return; }

            var client = new GithubService(value.userAgent, value.owner, value.repo, value.token);

            #region 不存在项目就创建
            var exists = await client.ReposCreateIfNotExistsAsync(value.repo);

            if (!exists.IsSuccessStatusCode)
            {
                var ReasonPhrase = exists.ReasonPhrase;

                var ResponseMessage = await exists.Content.ReadAsStringAsync();

                log.WriteLine(ReasonPhrase + ":" + ResponseMessage);
            }
            #endregion

            var contentResult = await client.ContentGetAsync("docs");

            #region 不存在Read the docs配置就创建
            if (!contentResult.IsSuccessStatusCode)
            {
                var readthedocsPath = Environment.CurrentDirectory + @"\Templates\readthedocs";

                var initFiles = Directory.GetFiles(readthedocsPath);

                var keywords = new Dictionary<string, string>()
            {
               { "<%repo%>",client.repo },
               { "<%owner%>",client.owner},
               { "<%year%>",DateTime.Now.Year.ToString()},
            };

                foreach (var file in initFiles)
                {
                    var content = string.Empty;

                    using (var reader = new StreamReader(file))
                    {
                        content = await reader.ReadToEndAsync();
                    }

                    foreach (var k in keywords)
                    {
                        content = content.Replace(k.Key, k.Value);
                    }

                    var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));

                    var fileName = Path.GetFileName(file);

                    var result = await client.FilePostAsync("docs/" + fileName, base64Content);

                    if (!result.IsSuccessStatusCode)
                    {
                        var ReasonPhrase = result.ReasonPhrase;

                        var ResponseMessage = await result.Content.ReadAsStringAsync();

                        log.WriteLine(ReasonPhrase + ":" + ResponseMessage);
                    }
                }

                contentResult = await client.ContentGetAsync("docs");
            }
            #endregion

            #region 生成的微服务文档
            var msdocName = md5String(value.swaggerUrl);
            var msdoc = string.Empty;
            var msdocPath = "docs/" + msdocName + ".rst";

            try
            {
                using (var hc = new HttpClient())
                {
                    var body = new StringContent(JsonConvert.SerializeObject(new
                    {
                        genName = "readthedocs.gen",
                        value.swaggerUrl
                    }), Encoding.UTF8, "application/json");

                    var result = await hc.PostAsync(ConfigurationManager.AppSettings["identityserver"] + "/CodeGen/Gen", body);

                    if (result.IsSuccessStatusCode)
                    {
                        var resultString = await result.Content.ReadAsStringAsync();

                        var resultJson = JsonConvert.DeserializeObject<JObject>(resultString);

                        if (resultJson["code"].Value<int>() == 200)
                        {
                            var data = resultJson["data"].Value<string>();

                            msdoc = Convert.ToBase64String(Encoding.UTF8.GetBytes(data));
                        }
                        else
                        {
                            log.WriteLine(resultString);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLine("生成微服务文档失败:" + ex.Message);
            }
            #endregion

            var contentResultStr = await contentResult.Content.ReadAsStringAsync();

            var items = JsonConvert.DeserializeObject<List<GithubFile>>(contentResultStr);

            #region 创建或更新微服务文档
            HttpResponseMessage msdocResult = null;
            var msdocFileName = msdocName + ".rst";
            var msdocItem = items.FirstOrDefault(x => x.name.Equals(msdocFileName));
            if (msdocItem != null)
            {
                msdocResult = await client.FilePutAsync(msdocPath, msdoc, msdocItem.sha);
            }
            else
            {
                msdocResult = await client.FilePostAsync(msdocPath, msdoc);
            }
            #endregion

            #region 加入或更新索引
            var file_Index = items.FirstOrDefault(x => x.name.Equals("index.rst"));
            var file_Index_Content = string.Empty;
            using (var hc = new HttpClient())
            {
                file_Index_Content = await hc.GetStringAsync(file_Index.download_url);
            }

            if (!file_Index_Content.Contains(msdocName))
            {
                var startPostion = "开放能力\r\n\r\n";
                var lineSymbol = "\r\n";
                if (file_Index_Content.IndexOf(startPostion) <= -1)
                {
                    startPostion = "开放能力\n\n";
                    lineSymbol = "\n";
                }

                file_Index_Content = file_Index_Content.Replace(startPostion, startPostion + "   " + msdocName + lineSymbol);

                var data = Encoding.UTF8.GetBytes(file_Index_Content);

                file_Index_Content = Convert.ToBase64String(data);

                var putResult = await client.FilePutAsync(file_Index.path, file_Index_Content, file_Index.sha);

                if (!putResult.IsSuccessStatusCode)
                {
                    var ReasonPhrase = putResult.ReasonPhrase;

                    var ResponseMessage = await putResult.Content.ReadAsStringAsync();

                    log.WriteLine(ReasonPhrase + ":" + ResponseMessage);
                }
            }
            #endregion
        }

        static string md5String(string str)
        {
            var md5 = new MD5CryptoServiceProvider();
            var bs = Encoding.UTF8.GetBytes(str);
            bs = md5.ComputeHash(bs);
            var s = new StringBuilder();
            foreach (byte b in bs)
            {
                s.Append(b.ToString("x2").ToUpper());
            }
            var password = s.ToString();
            return password;
        }
    }
}
