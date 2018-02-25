using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.MicroService.Models.Apis.UserController
{
    public class UserGetRequest
    {
        /// <summary>
        /// 用户角色
        /// </summary>
        public List<int> roles { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        [Phone(ErrorMessage = "手机号码格式错误")]
        public string phoneNumber { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [EmailAddress(ErrorMessage = "邮箱格式错误")]
        public string email { get; set; }
    }
}
