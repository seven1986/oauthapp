using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace OAuthApp.Services
{
    public class UploadService
    {
        private readonly IWebHostEnvironment _env;

        public UploadService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool Upload(string savePath, IFormFile file)
        {
            var path = Path.Combine(_env.WebRootPath, "blobs", savePath);

            var fileDorectory = Path.GetDirectoryName(path);

            if (!Directory.Exists(fileDorectory))
            {
                Directory.CreateDirectory(fileDorectory);
            }

            using var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);

            file.CopyTo(fs);

            return true;
        }
    }
}
