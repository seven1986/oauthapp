using OAuthApp.Jobs.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace OAuthApp.Jobs
{
    public class JobFunctions
    {
        readonly static AzureStorageService storage = new AzureStorageService();

        readonly static EmailService mail = new EmailService();

        /// <summary>
        /// 发布NPM包
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static void NpmPackagePublish(string packageUrl, TextWriter log)
        {
            // 解压文件夹名称
            var packageRootPath = AppDomain.CurrentDomain.BaseDirectory + "packages";

            if (!Directory.Exists(packageRootPath))
            {
                Directory.CreateDirectory(packageRootPath);
            }

            // 压缩包名
            var packageID = Guid.NewGuid().ToString("N");

            var packageName = packageID + ".zip";

            // zip包下载文件夹路径
            var packagePath = Path.Combine(packageRootPath, packageName);

            using (var client = new WebClient())
            {
                client.DownloadFileTaskAsync(new Uri(packageUrl), packagePath).Wait();
            }

            var packageExtractPath = Path.Combine(packageRootPath, packageID);

            ZipFile.ExtractToDirectory(packagePath, packageExtractPath);

            File.Delete(packagePath);

            if (Directory.GetFiles(packageExtractPath).Length < 1)
            {
                Directory.Delete(packageExtractPath);

                return;
            }

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
                    log.WriteLine(e.Data);
                    Console.WriteLine(e.Data);
                };

                p.ErrorDataReceived += (s, e) =>
                {
                    log.WriteLine(e.Data);
                    Console.WriteLine(e.Data);
                };

                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                using (var sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("cd /d " + packageExtractPath);
                        sw.WriteLine("npm publish");
                        //sw.WriteLine("cd  ..");
                        //sw.WriteLine("rmdir " + packageExtractPath + " /s");
                        //sw.WriteLine("Y");
                        //sw.WriteLine("rm " + packageName + " -f");
                    }
                }

                p.WaitForExit();
            }

            Directory.Delete(packageExtractPath, true);
        }
    }
}
