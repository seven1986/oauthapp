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
dotnet ef migrations add UpdateIdentityServerPersistedGrantDbMigration -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb
dotnet ef migrations add UpdateIdentityServerConfigurationDbMigration -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb
dotnet ef migrations add UpdateIdentityDbMigration -c IdentityDbContext -o Data/Migrations/Identity
dotnet ef migrations add UpdateTenantDbMigration -c TenantDbContext -o Data/Migrations/Tenant
```