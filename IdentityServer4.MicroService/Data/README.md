# Introduction 

##### 生成数据库迁移代码，需要注释Startup.cs里的下面两句话
```
1，InitialDBConfig.InitializeDatabase(app);
2，app.UseMutitenancy();
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

##### 生成数据库视图
将视图的脚本创建同名的Class，附加到InitialIdentityDbMigration类里

#### 其他
```
2018-1-2
1,ApiResource配置问题 https://github.com/IdentityServer/IdentityServer4/issues/1933
暂时保存在redis里面
```