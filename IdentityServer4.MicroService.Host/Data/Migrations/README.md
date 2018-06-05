# Introduction 

##### 首先要注释Startup.cs里的下面代码！
```
1，InitialDBConfig.InitializeDatabase(app);
2，app.UseIdentityServer4MicroService();
```

##### 首次生成迁移代码
```cmd
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add InitialIdentityDbMigration -c IdentityDbContext -o Data/Migrations/Identity
dotnet ef migrations add InitialTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
```

##### 更新迁移代码
```cmd
{datetime}替换成当前时间
dotnet ef migrations add {yyyyMMdd}UpdateIdentityDbMigration -c IdentityDbContext -o Data/Migrations/Identity
dotnet ef migrations add {yyyyMMdd}UpdateTenantDbMigration -c MutitenancyDbContext -o Data/Migrations/Tenant
```