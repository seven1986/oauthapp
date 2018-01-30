# IdentityServer4.MicroService

IdentityServer4.MicroService是一个免费开源的微服务框架。基于IdentityServer4、Azure API Management、Azure其他产品等构建。 目前主要由[seven1986](https://github.com/seven1986)创建和维护，它集成了IdentityServer4（令牌颁发、身份验证、单点登录和API访问控制所需的所有协议实现和扩展点），Azure API Management（集中管理所有API，配置访问策略、频次，生成文档与SDK）和其他主流技术。 


#### * 统一发布到AzureAPIManagement，自动生成文档、Server/Client端的SDK、SDK包并推送到各语言包管理平台，订阅并接受通知
示例：https://portal.ixingban.com/docs/services/5/operations/5a4c9af4882690135c8701d4
![sdkgen0](https://jixiucampaignstaging.blob.core.chinacloudapi.cn/adminportalicon/apisdkgen0.png)
![sdkgen](https://jixiucampaignstaging.blob.core.chinacloudapi.cn/adminportalicon/apisdkgen.png)

#### * 通过Azure API Management网关监控网关级日志，通过ELK监控业务级日志
![apilog1](https://jixiucampaignstaging.blob.core.chinacloudapi.cn/adminportal/apilog1.png)
![apilog2](https://jixiucampaignstaging.blob.core.chinacloudapi.cn/adminportal/apilog2.png)


### Acknowledgements
  IdentityServer4.MicroService is built using the following great open source projects
  
* [IdentityServer4](https://github.com/IdentityServer)
* [ASP.NET Core](https://github.com/aspnet)
* [Azure API Management](https://azure.microsoft.com/zh-cn/services/api-management/)
* [Swagger Codegen](https://github.com/swagger-api/swagger-codegen)


#### For run this project requires

* Azure Key Valut (统一配置，将Appsetting的配置、SSL证书迁移到Azure Key Valut等)
* Azure Redis （缓存）
* Azure SqlServer （持久存储，User、Client、ApiResource等数据）
* Azure Storage （Table/Queue/Blob）
* Email & Message （Send Cloud）
* Elastic Search （请求日志）
