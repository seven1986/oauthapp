using System;

namespace IdentityServer4.MicroService.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PolicyClaimValuesAttribute : Attribute
    {
        public string[] ClaimsValues { get; set; }

        /// <summary>
        /// 默认权限，用户注册后将获取该权限
        /// </summary>
        public bool IsDefault { get; set; }

        public PolicyClaimValuesAttribute(bool isDefault, params string[] claimsValues)
        {
            IsDefault = isDefault;
            ClaimsValues = claimsValues;
        }

        public PolicyClaimValuesAttribute(params string[] ClaimsValues) : this(false, ClaimsValues)
        {

        }
    }
}
