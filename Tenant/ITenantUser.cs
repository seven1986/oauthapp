using OAuthApp.Data;

namespace OAuthApp.Tenant
{
    public interface ITenantUser
    {
        TenantUserModel FindUser(string userName,string pwd);

        TenantUserModel FindUserByMobile(string mobile);

        TenantUserModel FindUserByEmail(string email);

        TenantUserModel FindUserByUnionID(long appID, string platform,string unionID);

        TenantUserModel FindUserByID(long ID);
    }
}
