using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace OAuthApp
{
    //¸üÐÂ dev ssl dotnet dev-certs https --trust
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
