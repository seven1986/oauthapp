using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.MicroService
{
    public class AppUser : IdentityUser<long>
    {
        // 也是唯一的
        public Guid UserKey { get; set; } = Guid.NewGuid();

        public long ParentUserID { get; set; }

        //当使用efcore重新生成脚本时，取消下面的注释
        //这样生成的脚本会设置该字段对应sqlserver的sys.hierarchyid类型
        //脚本生成好以后，注释掉column属性，换成notmapped属性
        //[Column(TypeName = "sys.hierarchyid")]
        //如果不需要生成脚本，请需要注释
        [NotMapped]
        public string Lineage { get; set; }

        public string LineageIDs { get; set; }

        public string Avatar { get; set; }

        /// <summary>
        /// 是否已删除账号
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Navigation property for the roles this user belongs to.
        /// </summary>
        public virtual List<AppUserRole> Roles { get; } = new List<AppUserRole>();

        /// <summary>
        /// Navigation property for the claims this user possesses.
        /// </summary>
        public virtual List<AppUserClaim> Claims { get; } = new List<AppUserClaim>();

        /// <summary>
        /// Navigation property for this users login accounts.
        /// </summary>
        public virtual List<AppUserLogin> Logins { get; } = new List<AppUserLogin>();

        /// <summary>
        /// Navigation property for this users token accounts.
        /// </summary>
        public virtual List<AppUserToken> Tokens { get; } = new List<AppUserToken>();

        /// <summary>
        /// 租户
        /// </summary>
        public virtual List<AspNetUserTenant> Tenants { get; } = new List<AspNetUserTenant>();

        /// <summary>
        /// files
        /// </summary>
        public virtual List<AspNetUserFile> Files { get; } = new List<AspNetUserFile>();

        /// <summary>
        /// Clients
        /// </summary>
        public virtual List<AspNetUserClient> Clients { get; } = new List<AspNetUserClient>();

        /// <summary>
        /// ApiResources
        /// </summary>
        public virtual List<AspNetUserApiResource> ApiResources { get; } = new List<AspNetUserApiResource>();

        /// <summary>
        /// Distribution
        /// </summary>
        public virtual List<AspNetUserDistribution> Distributions { get; } = new List<AspNetUserDistribution>();

        /// <summary>
        /// 分类
        /// </summary>
        public string TypeIDs { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// 身高
        /// </summary>
        public decimal Stature { get; set; }

        /// <summary>
        /// 体重
        /// </summary>
        public decimal Weight { get; set; }

        /// <summary>
        /// 介绍
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// 最后更新日期
        /// </summary>
        public DateTime LastUpdateTime { get; set; }
    }

    #region identity
    public class AppRole : IdentityRole<long>
    {
        /// <summary>
        /// Navigation property for the claims this role belongs to.
        /// </summary>
        public virtual List<AppRoleClaim> Claims { get; } = new List<AppRoleClaim>();
    }
    public class AppRoleClaim : IdentityRoleClaim<long>
    {
    }
    public class AppUserRole : IdentityUserRole<long>
    {
    }
    public class AppUserClaim : IdentityUserClaim<long>
    {
    }
    public class AppUserLogin : IdentityUserLogin<long>
    {
    }
    public class AppUserToken : IdentityUserToken<long>
    {
    }

    public enum FileTypes
    {
        Image = 0,
        Video = 1,
        Doc = 2
    }
    [Table("AspNetUserFiles")]
    public class AspNetUserFile
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// 0图片
        /// 1视频
        /// 2文档
        /// </summary>
        public FileTypes FileType { get; set; }

        public string Files { get; set; }
    }

    /// <summary>
    ///  relation between User and Tenant
    /// </summary>
    [Table("AspNetUserTenants")]
    public class AspNetUserTenant
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        // 外键
        public long TenantId { get; set; }
    }

    /// <summary>
    /// relation between User and Client
    /// </summary>
    [Table("AspNetUserClients")]
    public class AspNetUserClient
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        // 外键
        public long ClientId { get; set; }
    }

    /// <summary>
    /// relation between User and ApiResource
    /// </summary>
    [Table("AspNetUserApiResources")]
    public class AspNetUserApiResource
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        // 外键
        public long ApiResourceId { get; set; }
    }

    /// <summary>
    /// Distribution
    /// </summary>
    [Table("AspNetUserDistribution")]
    public class AspNetUserDistribution
    {
        public long Id { get; set; }

        // 外键
        public long TenantId { get; set; }

        public long UserId { get; set; }

        /// <summary>
        /// Members Count
        /// </summary>
        public long Members { get; set; }

        /// <summary>
        /// MembersLastUpdate
        /// </summary>
        public DateTime MembersLastUpdate { get; set; }

        /// <summary>
        /// Sale Amount
        /// </summary>
        public decimal Sales { get; set; }

        /// <summary>
        /// SalesLastUpdate
        /// </summary>
        public DateTime SalesLastUpdate { get; set; }

        /// <summary>
        /// Earned
        /// </summary>
        public decimal Earned { get; set; }

        /// <summary>
        /// EarnedDiff
        /// </summary>
        public decimal EarnedDiff { get; set; }

        /// <summary>
        /// EarnedDiffLastUpdate
        /// </summary>
        public DateTime EarnedDiffLastUpdate { get; set; }

        /// <summary>
        /// Commission
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// CommissionLastUpdate
        /// </summary>
        public DateTime CommissionLastUpdate { get; set; }

        /// <summary>
        /// CommissionLv1
        /// </summary>
        public decimal CommissionLv1 { get; set; }
        /// <summary>
        /// CommissionLv1LastUpdate
        /// </summary>
        public DateTime CommissionLv1LastUpdate { get; set; }

        /// <summary>
        /// CommissionLv2
        /// </summary>
        public decimal CommissionLv2 { get; set; }
        /// <summary>
        /// CommissionLv2LastUpdate
        /// </summary>
        public DateTime CommissionLv2LastUpdate { get; set; }

        /// <summary>
        /// CommissionLv3
        /// </summary>
        public decimal CommissionLv3 { get; set; }
        /// <summary>
        /// CommissionLv3LastUpdate
        /// </summary>
        public DateTime CommissionLv3LastUpdate { get; set; }
    }
    #endregion

    #region 数据库视图 View_User 对应的实体
    [NotMapped]
    public class View_User
    {
        public long AppTenantId { get; set; }
        public long UserId { get; set; }
        public List<View_User_Role> Roles { get; set; }
        public List<View_User_Claim> Claims { get; set; }
        public List<View_User_File> Files { get; set; }
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Lineage { get; set; }
        public long ParentUserID { get; set; }
        public string ParentUserName { get; set; }
        public string TypeIDs { get; set; }
        public string PasswordHash { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public long DistributionID { get; set; }
        public long Members { get; set; }
        public DateTime MembersLastUpdate { get; set; }
        public decimal Sales { get; set; }
        public DateTime SalesLastUpdate { get; set; }
        public decimal Earned { get; set; }
        public decimal EarnedDiff { get; set; }
        public DateTime EarnedDiffLastUpdate { get; set; }
        public decimal Commission { get; set; }
        public DateTime CommissionLastUpdate { get; set; }
        public decimal CommissionLv1 { get; set; }
        public DateTime CommissionLv1LastUpdate { get; set; }
        public decimal CommissionLv2 { get; set; }
        public DateTime CommissionLv2LastUpdate { get; set; }
        public decimal CommissionLv3 { get; set; }
        public DateTime CommissionLv3LastUpdate { get; set; }
    }
    /// <summary>
    /// used for View_User Roles property
    /// </summary>
    public class View_User_Role
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }
    }

    /// <summary>
    /// used for View_User Claims property
    /// </summary>
    public class View_User_Claim
    {
        public long Id { get; set; }

        public string ClaimType { get; set; }

        public string ClaimValue { get; set; }
    }

    /// <summary>
    /// used for View_User Files property
    /// </summary>
    public class View_User_File
    {
        public long Id { get; set; }

        public int FileType { get; set; }

        public string Files { get; set; }
    }
    #endregion
}
