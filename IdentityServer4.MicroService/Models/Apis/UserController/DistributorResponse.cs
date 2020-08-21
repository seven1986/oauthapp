using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    [NotMapped]
    public class DistributorResponse
    {
        public long ID { get; set; }
        public string UserName { get; set; }

        public string NickName { get; set; }

        public string Avatar { get; set; }

        public long Members { get; set; }

        public decimal Sales { get; set; }
        public long ParentUserID { get; set; }

        public short LineageLevel { get; set; }

        public string Lineage { get; set; }
    }
}
