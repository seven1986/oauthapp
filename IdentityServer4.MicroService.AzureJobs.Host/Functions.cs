using IdentityServer4.MicroService.AzureJobs.Services;
using Microsoft.Azure.WebJobs;
using System.IO;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.AzureJobs.Host
{
    public class Functions
    {
        /// <summary>
        /// 发布NPM包
        /// </summary>
        /// <param name="packageUrl"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ReleasePackage_NPM([QueueTrigger("publish-package-npm")] string packageUrl, TextWriter log)
        {
           await Fns.ReleasePackage_NPM(packageUrl, log);
        }

        /// <summary>
        /// 发送邮件通知给订阅者
        /// </summary>
        /// <param name="apiId"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ApiresourcePublishNotify([QueueTrigger("apiresource-publish")] string apiId, TextWriter log)
        {
            await Fns.ApiresourcePublishNotify(apiId, log);
        }

        /// <summary>
        /// 同步Github标签
        /// </summary>
        /// <param name="value"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ApiresourcePublishGithub([QueueTrigger("apiresource-publish-github")] GithubQueueModel value, TextWriter log)
        {
            await Fns.ApiresourcePublishGithub(value, log);
        }

        /// <summary>
        /// 初始化Github项目中readthedocs的配置
        /// </summary>
        /// <param name="value"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        public static async Task ApiresourcePublishGithubReadthedocs([QueueTrigger("apiresource-publish-github-readthedocs")] GithubQueueModel value, TextWriter log)
        {
            await Fns.ApiresourcePublishGithubReadthedocs(value, log);
        }
    }
}
