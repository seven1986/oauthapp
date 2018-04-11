using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;

namespace IdentityServer4.MicroService.SDKGen
{
    public class Functions
    {
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

                client.DownloadFile(packageUrl, packagePath);

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
    }
}
