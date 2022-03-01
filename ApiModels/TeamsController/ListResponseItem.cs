using System;

namespace OAuthApp.ApiModels.TeamsController
{
    public class ListResponseItem
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }
        public string Permission { get; set; }
        public DateTime LastUpdate { get; set; }
    }
}
