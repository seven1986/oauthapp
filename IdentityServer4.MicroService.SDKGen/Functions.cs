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
        // This function will get triggered/executed when a new message is written 
        // on an Azure Queue called queue.
        public static async Task ProcessQueueMessageAsync([QueueTrigger("queue")] string message, TextWriter log)
        {
            log.WriteLine(message);

            using (var client = new WebClient())
            {
                client.DownloadFile(new Uri(message), "D:\\home\\data\\jobs\\npm\\npmpackage.zip");

                ZipFile.ExtractToDirectory("D:\\home\\data\\jobs\\npm\\npmpackage.zip", "D:\\home\\data\\jobs\\npm\\npmpackage");

                Process p = new Process();
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "cmd.exe";
                info.RedirectStandardInput = true;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;
                info.UseShellExecute = false;

                p.StartInfo = info;
                p.OutputDataReceived += (s, e) => log.WriteLine(e.Data);
                p.ErrorDataReceived += (s, e) => log.WriteLine(e.Data);
                p.Start();
                p.BeginOutputReadLine();
                p.BeginErrorReadLine();

                using (StreamWriter sw = p.StandardInput)
                {
                    if (sw.BaseStream.CanWrite)
                    {
                        sw.WriteLine("cd  D:\\home\\data\\jobs\\npm\\npmpackage");
                        sw.WriteLine("npm publish");
                        sw.WriteLine("cd  ..");
                        sw.WriteLine("rmdir npmpackage /s");
                        sw.WriteLine("Y");
                        sw.WriteLine("rm npmpackage.zip -f");
                    }
                }

                p.WaitForExit();
            }
        }
    }
}
