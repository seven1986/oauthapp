using System;

namespace IdentityServer4.MicroService.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class PolicyClaimValuesAttribute : Attribute
    {
        public string[] PolicyValues { get; set; }

        public string ControllerName { get; set; }

        /// <summary>
        /// 默认权限，用户注册后将获取该权限
        /// </summary>
        public bool IsDefault { get; set; }

        public PolicyClaimValuesAttribute(bool isDefault, string controllerName, params string[] policyValues)
        {
            IsDefault = isDefault;
            PolicyValues = policyValues;
            ControllerName = controllerName;
        }

        public PolicyClaimValuesAttribute(params string[] policyValues) : this(false, string.Empty, policyValues)
        {

        }

        public PolicyClaimValuesAttribute(string controllerName, params string[] policyValues) : this(false, controllerName, policyValues)
        {

        }
    }
}
