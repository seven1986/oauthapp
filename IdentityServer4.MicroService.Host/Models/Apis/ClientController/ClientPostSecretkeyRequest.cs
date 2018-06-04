using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.MicroService.Models.Apis.ClientController
{
    public class ClientPostSecretkeyRequest
    {
        public SecretKeyType keyType { get; set; } = SecretKeyType.Sha512;

        /// <summary>
        /// 密钥明文
        /// </summary>
        [Required(ErrorMessage = "密钥明文不能为空")]
        public string plaintext { get; set; }
    }

    public enum SecretKeyType
    {
        Sha256=0,
        Sha512=1,
    }
}
