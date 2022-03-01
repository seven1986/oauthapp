using System;

namespace OAuthApp.Models
{
    public class AuthorizationCode
    {
        public long tenantId { get; set; }

        public string tenantName { get; set; }

        public string client_id { get; set; }

        public long timestamp { get; set; } = DateTime.UtcNow.Ticks;

        public string redirect_uri { get; set; }

        public string response_type { get; set; }

        public string userName { get; set; }

        public string userRole { get; set; }

        public long userId { get; set; }

        public string scope { get; set; }

        public string request_uri { get; set; }

        public string userAvatar { get; set; }
    }
}
