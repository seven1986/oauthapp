using OAuthApp.Data;
using System;
using System.Linq;

namespace OAuthApp.Tenant
{
    public class TenantUser : ITenantUser
    {
        private readonly AppDbContext _appDb;
        
        public TenantUser(AppDbContext appDb)
        {
            _appDb = appDb;
        }

        public TenantUserModel FindUserByEmail(string email)
        {
            var user = _appDb.Users.FirstOrDefault(x => x.Email == email);

            return GetTenantUser(user);
        }

        public TenantUserModel FindUserByID(long ID)
        {
            var user = _appDb.Users.FirstOrDefault(x => x.ID == ID);

            return GetTenantUser(user);
        }

        public TenantUserModel FindUserByMobile(string mobile)
        {
            var user = _appDb.Users.FirstOrDefault(x => x.Phone == mobile);

            return GetTenantUser(user);
        }

        public TenantUserModel FindUser(string userName,string pwd)
        {
            var user = _appDb.Users.FirstOrDefault(x => x.UserName == userName);

            if (user == null)
            {
                throw new ArgumentException("用户不存在");
            }
            else if (!user.Password.Equals(pwd))
            {
                throw new ArgumentException("密码错误");
            }

            return GetTenantUser(user);
        }

        public TenantUserModel FindUserByUnionID(long appID,string platform, string unionID)
        {
            var userId = _appDb.AppUsers
                .Where(x => x.AppID == appID && x.Platform == platform && x.UnionID == unionID)
                .Select(x => x.UserID).FirstOrDefault();

            return FindUserByID(userId);
        }

        TenantUserModel GetTenantUser(User user)
        {
            if (user == null)
            {
                return null;
            }

            var role = _appDb.UserClaims
                .Where(x => x.UserID == user.ID && x.ClaimType.Equals("role"))
                .Select(x => x.ClaimValue).FirstOrDefault();

            return new TenantUserModel()
            {
                ID = user.ID,
                Name = user.UserName,
                NickName = user.NickName ?? "",
                Email = user.Email ?? "",
                Mobile = user.Phone ?? "",
                Avatar = user.Avatar ?? "",
                Role = role ?? ""
            };
        }
    }
}
