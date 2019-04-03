﻿概述
====


    基于Identityserver4、Azure API Management的开放平台解决方案。


微服务
--------

   WebApi项目，可调用的接口。（通常以业务模块划分，如：订单、资讯、商品）

   - 接口文档
        
        接口说明，让使用者能明白接口提供功能。

   - 接口SDK
    
        调用接口的编程语言代码。


Azure API 网关
--------

 聚合WebApi的网络关卡，保障安全性、提供一致性访问、开放性的产品。

 详见：https://www.azure.cn/zh-cn/home/features/api-management

产品
-----

    组合多个微服务，以产品包的形式提供使用

  -   产品文档

        应包含产品概述、接入规范、使用说明、接口文档、常见问题、代码示例等。

  -   产品脚手架

        应包含产品接口SDK的命令行工具。快速生成基于产品的seed项目，减少项目初始化工作。

        常见形式：angular cli、vue cli、react cli 等等

  -   产品控制台

        应包含产品设置、后台管理的功能。

        
