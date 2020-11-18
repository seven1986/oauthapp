# 使用说明

##### 首先要注释掉Startup.cs文件中下面2行代码！
```
1，在引用的web项目安装Microsoft.EntityFrameworkCore.Design、Microsoft.EntityFrameworkCore.Tools
2，修改OAuthAppServiceBuilderExtensions.cs文件的opts.MigrationsAssembly为web项目名称
3，修改IdentityTables.cs文件AppUser类的Lineage属性
4，定位到web项目执行执行【首次生成迁移代码】或者【更新迁移代码】
5，拷贝生成的文件到Data目录下
6，修改ISMSServiceBuilderExtensions.cs文件的opts.MigrationsAssembly为 OAuthApp
7，移除web项目的Microsoft.EntityFrameworkCore.Design引用
8，还原IdentityTables.cs文件AppUser类的Lineage属性
```

##### 首次生成迁移代码
```javaScript
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add InitialUserDbMigration -c UserDbContext -o Data/Migrations/User
dotnet ef migrations add InitialTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
dotnet ef migrations add InitialSDKDbMigration -c SDKDbContext -o Data/Migrations/SDK

InitialUserDbMigration.cs 文件
 - Up方法添加 migrationBuilder.Sql(SQL_View_User.SQL);
 - Down方法添加 migrationBuilder.DropTable(SQL_View_User.Name);
```

##### 更新迁移代码

```javaScript
// {datetime}替换为现在的时间，例如：20181207
dotnet ef migrations add {datetime}UpdateIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add {datetime}UpdateIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add {datetime}UpdateIdentityDbMigration -c IdentityDbContext -o Data/Migrations/Identity
dotnet ef migrations add {datetime}UpdateTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
dotnet ef migrations add {datetime}UpdateSDKDbMigration -c SDKDbContext -o Data/Migrations/SDK
```