# 使用说明

##### 首先要注释掉Startup.cs文件中下面2行代码！
```
1，在引用的web项目安装Microsoft.EntityFrameworkCore.Design
2，修改ISMSServiceBuilderExtensions.cs文件的opts.MigrationsAssembly为web项目名称
3，定位到web项目执行执行【首次生成迁移代码】或者【更新迁移代码】
4，拷贝生成的文件到Data目录下
5，修改ISMSServiceBuilderExtensions.cs文件的opts.MigrationsAssembly为IdentityServer4.MicroService
6，移除web项目的Microsoft.EntityFrameworkCore.Design引用
```

##### 首次生成迁移代码
```javaScript
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add InitialUserDbMigration -c UserDbContext -o Data/Migrations/User
dotnet ef migrations add InitialTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
```

##### 更新迁移代码

```javaScript
// {datetime}替换为现在的时间，例如：20181207
dotnet ef migrations add {datetime}UpdateIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add {datetime}UpdateIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add {datetime}UpdateIdentityDbMigration -c IdentityDbContext -o Data/Migrations/Identity
dotnet ef migrations add {datetime}UpdateTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
```