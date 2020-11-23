namespace OAuthApp.Data
{
    public class SQL_View_User
    {
        public const string Name = "View_User";

        public const string SQL = @"CREATE VIEW [dbo].[View_User]
AS
SELECT   A.ParentUserID,A.NickName as UserName, A.Avatar, A.CreateDate, A.LockFlag, A.DataAmount, A.Status, A.TypeIDs, B.ID, B.UserID, 
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
				B.CommissionLv3LastUpdate, 
				A.PasswordHash AS UserPwd, 
                A.Lineage.ToString() AS Lineage, C.UserName AS ParentUserName,
				
			  A.Email,
			  A.EmailConfirmed,

			  A.PhoneNumber,
			  A.PhoneNumberConfirmed,

			  (SELECT Q2.Id, Q2.Name, Q2.NormalizedName FROM AspNetUserRoles Q1
			  JOIN AspNetRoles Q2 ON Q1.RoleId = Q2.Id
			  WHERE UserId = A.Id FOR JSON AUTO) as Roles,
			  
			  (SELECT Q1.Id, Q1.ClaimType,Q1.ClaimValue FROM AspNetUserClaims Q1
			  WHERE Q1.UserId = A.Id FOR JSON AUTO) as Claims,
			  
			  (SELECT Q1.Id, Q1.Files,Q1.FileType AS Name FROM AspNetUserFiles Q1
			  WHERE Q1.UserId = A.Id FOR JSON AUTO) as Files,
			  
			  (SELECT Q1.Id, Q1.[Key],Q1.[Value] FROM AspNetUserProperties Q1
			  WHERE Q1.UserId = A.Id FOR JSON AUTO) as Properties,

			  (SELECT Q1.LoginProvider, Q1.ProviderKey,Q1.ProviderDisplayName FROM AspNetUserLogins Q1
			  WHERE Q1.UserId = A.Id FOR JSON AUTO) as Logins,

			  (SELECT Q1.TenantId FROM AspNetUserTenants Q1
			  WHERE Q1.UserId = A.Id FOR JSON AUTO) as Tenants

FROM      dbo.AspNetUsers AS A INNER JOIN
                dbo.AspNetUserDistributors AS B ON A.ID = B.UserID INNER JOIN
                dbo.AspNetUsers AS C ON A.ParentUserID = C.ID";
    }
}
