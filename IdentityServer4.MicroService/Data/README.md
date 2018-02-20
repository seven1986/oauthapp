# Introduction 

##### InitialDB
```
// 执行命令前，先注释掉
1，InitialDBConfig.InitializeDatabase(app);
2，app.UseMutitenancy();
```

```cmd
dotnet ef migrations add InitialIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add InitialIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add InitialIdentityDbMigration -c ApplicationDbContext -o Data/Migrations/Identity
dotnet ef migrations add InitialTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
```


##### UpdateDB
```
{datetime}替换成当前时间
dotnet ef migrations add {yyyyMMdd}UpdateIdentityDbMigration -c ApplicationDbContext -o Data/Migrations/Identity
dotnet ef migrations add {yyyyMMdd}UpdateTenantDbMigration -c MutitenancyDbContext -o Data/Migrations/Tenant
```

```
2018-1-2
1,ApiResource配置问题 https://github.com/IdentityServer/IdentityServer4/issues/1933
暂时保存在redis里面
```