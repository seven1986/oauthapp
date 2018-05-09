using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.MicroService.AzureJobs.Models;
using IdentityServer4.MicroService.AzureJobs.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Table;

namespace IdentityServer4.MicroService.AzureJobs
{
    public class Functions
    {
        readonly static AzureStorageService storage = new AzureStorageService();

        readonly static EmailService mail = new EmailService();

        public static async Task ReleasePackage_NPM([QueueTrigger("publish-package-npm")] string packageUrl, TextWriter log)
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
                            sw.WriteLine("cd  "+packageExtractToDirectory);
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

        public static async Task ApiresourcePublishNotify([QueueTrigger("apiresource-publish")] string apiId, TextWriter log)
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
            for(var i=0;i<mails.Count;i++)
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
            catch(Exception ex)
            {
                log.WriteLine(ex.Message);
            }
        }
    }
}
