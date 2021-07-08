using IdentityServer4.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAuthApp.Models.Apis.LogController
{
    public class LogPostRequest
    {
        public string message { get; set; }

        public LogLevel level { get; set; } = LogLevel.Information;
    }
}
