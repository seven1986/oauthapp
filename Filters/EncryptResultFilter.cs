using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace OAuthApp.Filters
{
    public class EncryptResultFilterAttribute : TypeFilterAttribute
    {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatString = "yyyy-MM-dd HH:mm:ss"
        };

        static string Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            using (var aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Key =Key;
                aesAlg.IV = IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }

                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(encrypted);
        }

        public EncryptResultFilterAttribute() : base(typeof(EncryptResultFilter))
        {

        }

        public class EncryptResultFilter : IResultFilter
        {
            public EncryptResultFilter()
            {
            }

            public void OnResultExecuting(ResultExecutingContext context)
            {
                if (context.Result is JsonResult)
                {
                    var key = KEY;

                    var iv = IV;

                    var seed = Convert.ToBase64String(key) + "." + Convert.ToBase64String(iv);

                    var result = context.Result as JsonResult;

                    var str = JsonConvert.SerializeObject(result.Value, _settings);
                    
                    var data = Encrypt(str, key, iv);

                    context.Result = new JsonResult(new { __encrypt = true, seed, data });
                }
            }

            public byte[] KEY 
            {
                get
                {
                    var _KEY = DateTime.Now.Ticks.ToString().Substring(0, 16);

                    return Encoding.UTF8.GetBytes(_KEY);
                }
            }

            public byte[] IV
            {
                get
                {
                    var _IV = DateTime.Now.Ticks.ToString().Substring(0, 16);

                    return Encoding.UTF8.GetBytes(_IV);
                }
            }

            public void OnResultExecuted(ResultExecutedContext context)
            {
               
            }
        }
    }
}
