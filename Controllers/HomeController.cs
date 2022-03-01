using OAuthApp.Data;
using OAuthApp.Models;
using OAuthApp.Tenant;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace OAuthApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TenantDbContext _context;
        private readonly AppDbContext _appDbContext;

        public HomeController(
          ILogger<HomeController> logger,
          TenantDbContext context,
          AppDbContext appDbContext)
        {
            _logger = logger;
            _context = context;
            _appDbContext = appDbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var error = feature?.Error;

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StackTrace = error?.StackTrace,
                ErrorMessage = error?.Message
            });
        }
    }

    public class SignUpPostModel
    {
        [Required]
        [RegularExpression("[a-z0-9_-]{6,12}")]
        public string Domain { get; set; }

        [Required]
        [Phone]
        public string Mobile { get; set; }

        [Required]
        [RegularExpression("[0-9]{4,6}")]
        public string Code { get; set; }

        [Required]
        [RegularExpression("[0-9a-zA-Z_-]")]
        public string Pwd { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
