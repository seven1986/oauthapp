# 使用说明

##### 首先要注释掉Startup.cs文件中下面2行代码！
```
1，AppDefaultData.InitializeDatabase(app);
2，app.UseIdentityServer4MicroService();
```

##### 首次生成迁移代码
```javaScript
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add InitialIdentityDbMigration -c IdentityDbContext -o Data/Migrations/Identity
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