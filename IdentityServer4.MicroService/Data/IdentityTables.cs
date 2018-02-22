using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.MicroService
{
    public class AppUser : IdentityUser<long>
    {
        public long ParentUserID { get; set; }

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
        /// 流量余额
        /// </summary>
        public long DataAmount { get; set; }

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
    /*
CREATE VIEW dbo.View_User
AS
SELECT
 D.AppTenantId,
 A.Id as UserId,
(SELECT Q2.Id, Q2.Name, Q2.NormalizedName FROM AspNetUserRoles Q1
JOIN AspNetRoles Q2 ON Q1.RoleId = Q2.Id
WHERE UserId = A.Id FOR JSON AUTO) as Roles,

(SELECT Q1.ClaimType,Q1.ClaimValue FROM AspNetUserClaims Q1
WHERE Q1.UserId = A.Id FOR JSON AUTO) as Claims,

(SELECT Q1.Files,Q1.FileType AS Name FROM AspNetUserFiles Q1
WHERE Q1.AppUserId = A.Id FOR JSON AUTO) as Files,

 A.Avatar,
 A.UserName,
 A.Email,
 A.Lineage.ToString() AS Lineage, 
 A.ParentUserID, 
 C.UserName AS ParentUserName,
 A.DataAmount,
 A.TypeIDs, 
 A.PasswordHash, 
 A.IsDeleted,
 A.CreateDate, 
 A.LastUpdateTime,
 B.ID AS DistributionID, 
 B.Members, 
 B.MembersLastUpdate,
 B.Sales,
 B.SalesLastUpdate, 
 B.Earned, 
 B.EarnedDiff, 
 B.EarnedDiffLastUpdate, 
 B.Commission, 
 B.CommissionLastUpdate, 
 B.CommissionLv1, 
 B.CommissionLv1LastUpdate, 
 B.CommissionLv2, 
 B.CommissionLv2LastUpdate, 
 B.CommissionLv3, 
 B.CommissionLv3LastUpdate
 FROM AspNetUsers AS A
 INNER JOIN AspNetUserDistribution AS B ON A.ID = B.UserID
 INNER JOIN AspNetUsers AS C ON A.ParentUserID = C.ID
 INNER JOIN AspNetUserTenants AS D ON A.Id = D.AppUserId

 --SELECT RoleId FROM AspNetUserRoles WHERE UserId = 1 FOR JSON AUTO
     */
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
        public long DataAmount { get; set; }
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
