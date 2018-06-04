namespace IdentityServer4.MicroService.Data
{
    public class View_IdentityUser
    {
        public const string Name = "View_IdentityUser";

        public const string SQL = @"CREATE VIEW View_IdentityUser
AS
SELECT
 D.TenantId,
 A.Id as UserId,
(SELECT Q2.Id, Q2.Name, Q2.NormalizedName FROM AspNetUserRoles Q1
JOIN AspNetRoles Q2 ON Q1.RoleId = Q2.Id
WHERE UserId = A.Id FOR JSON AUTO) as Roles,

(SELECT Q1.Id, Q1.ClaimType,Q1.ClaimValue FROM AspNetUserClaims Q1
WHERE Q1.UserId = A.Id FOR JSON AUTO) as Claims,

(SELECT Q1.Id, Q1.Files,Q1.FileType AS Name FROM AspNetUserFiles Q1
WHERE Q1.UserId = A.Id FOR JSON AUTO) as Files,

(SELECT Q1.Id, Q1.[Key],Q1.[Value] FROM AspNetUserProperties Q1
WHERE Q1.UserId = A.Id FOR JSON AUTO) as Properties,

 A.Avatar,
 A.UserName,
 A.Email,
 A.Lineage.ToString() AS Lineage, 
 A.ParentUserID, 
 C.UserName AS ParentUserName,
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
 INNER JOIN AspNetUserDistributors AS B ON A.ID = B.UserID
 INNER JOIN AspNetUsers AS C ON A.ParentUserID = C.ID
 INNER JOIN AspNetUserTenants AS D ON A.Id = D.UserId";
    }
}
