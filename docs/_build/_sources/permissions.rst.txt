权限码表
=========

.. Note::

    client在请求标识中心颁发token时，默认支持下面的scope。


API
----------------------

.. code-block:: javascript
  
  [
 	{
 	 	"code": "isms.apiresource.publishrevision",
 	 	"name": "微服务 - 网关 - 创建修订版",
 	},
 	{
 	 	"code": "isms.apiresource.publishconfiguration",
 	 	"name": "微服务 - 网关 - 上次发布配置",
 	},
 	{
 	 	"code": "isms.apiresource.publish",
 	 	"name": "微服务 - 网关 - 发布或更新版本",
 	},
 	{
 	 	"code": "isms.apiresource.products",
 	 	"name": "微服务 - 网关 - 产品组",
 	},
 	{
 	 	"code": "isms.apiresource.postrelease",
 	 	"name": "微服务 - 修订内容 - 发布",
 	},
 	{
 	 	"code": "isms.apiresource.postpackages",
 	 	"name": "微服务 - 包市场 - 添加",
 	},
 	{
 	 	"code": "isms.apiresource.post",
 	 	"name": "微服务 - 创建",
 	},
 	{
 	 	"code": "isms.apiresource.publishversion",
 	 	"name": "微服务 - 网关 - 创建新版本",
 	},
 	{
 	 	"code": "isms.apiresource.packages",
 	 	"name": "微服务 - 包市场 - 列表",
 	},
 	{
 	 	"code": "isms.apiresource.get",
 	 	"name": "微服务 - 列表",
 	},
 	{
 	 	"code": "isms.apiresource.detail",
 	 	"name": "微服务 - 详情",
 	},
 	{
 	 	"code": "isms.apiresource.deleterelease",
 	 	"name": "微服务 - 修订内容 - 删除",
 	},
 	{
 	 	"code": "isms.apiresource.deletepackage",
 	 	"name": "微服务 - 包市场 - 删除",
 	},
 	{
 	 	"code": "isms.apiresource.delete",
 	 	"name": "微服务 - 删除",
 	},
 	{
 	 	"code": "isms.apiresource.authservers",
 	 	"name": "微服务 - 网关 - OAuthServers",
 	},
 	{
 	 	"code": "isms.apiresource.all",
 	 	"name": "微服务 - 所有权限",
 	},
 	{
 	 	"code": "isms.apiresource.history",
 	 	"name": "代码生成 - SDK - 发布记录",
 	},
 	{
 	 	"code": "isms.apiresource.put",
 	 	"name": "微服务 - 更新",
 	},
 	{
 	 	"code": "isms.apiresource.putrelease",
 	 	"name": "微服务 - 修订内容 - 更新",
 	},
 	{
 	 	"code": "isms.apiresource.putpackage",
 	 	"name": "微服务 - 包市场 - 更新",
 	},
 	{
 	 	"code": "isms.apiresource.versions",
 	 	"name": "微服务 - 网关 - 版本列表",
 	},
 	{
 	 	"code": "isms.apiresource.verifyemail",
 	 	"name": "微服务 - 订阅者 - 验证邮箱",
 	},
 	{
 	 	"code": "isms.apiresource.subscriptions",
 	 	"name": "微服务 - 订阅者 - 列表",
 	},
 	{
 	 	"code": "isms.apiresource.setonlineversion",
 	 	"name": "微服务 - 网关 - 上线指定版本",
 	},
 	{
 	 	"code": "isms.apiresource.scopes",
 	 	"name": "微服务 - 权限代码",
 	},
 	{
 	 	"code": "isms.apiresource.releases",
 	 	"name": "微服务 - 修订内容 - 列表",
 	}
]


Blob
----------------------

.. code-block:: javascript
  
  [
 	{
 	 	"code": "isms.blob.post",
 	 	"name": "文件 - 上传视频或文档",
 	},
 	{
 	 	"code": "isms.blob.image",
 	 	"name": "文件 - 上传图片",
 	},
 	{
 	 	"code": "isms.blob.base64",
 	 	"name": "文件 - 上传base64格式的png图片",
 	},
 	{
 	 	"code": "isms.blob.all",
 	 	"name": "文件 - 所有权限",
 	}
]

客户端
----------------------

.. code-block:: javascript
  
  [
 	{
 	 	"code": "isms.client.put",
 	 	"name": "客户端 - 更新",
 	},
 	{
 	 	"code": "isms.client.postsecretkey",
 	 	"name": "客户端 - 生成密钥",
 	},
 	{
 	 	"code": "isms.client.post",
 	 	"name": "客户端 - 创建",
 	},
 	{
 	 	"code": "isms.client.issuetoken",
 	 	"name": "客户端 - 创建令牌",
 	},
 	{
 	 	"code": "isms.client.get",
 	 	"name": "客户端 - 列表",
 	},
 	{
 	 	"code": "isms.client.detail",
 	 	"name": "客户端 - 详情",
 	},
 	{
 	 	"code": "isms.client.all",
 	 	"name": "客户端 - 所有权限",
 	},
 	{
 	 	"code": "isms.client.delete",
 	 	"name": "客户端 - 删除",
 	}
]

代码生成
----------------------

.. code-block:: javascript
  
  [
 	{
 	 	"code": "isms.codegen.syncgithub",
 	 	"name": "代码生成 - Github - 同步",
 	},
 	{
 	 	"code": "isms.codegen.servers",
 	 	"name": "代码生成 - 服务端列表",
 	},
 	{
 	 	"code": "isms.codegen.releasesdk",
 	 	"name": "代码生成 - SDK - 发布",
 	},
 	{
 	 	"code": "isms.codegen.putnpmoptions",
 	 	"name": "代码生成 - NPM - 更新设置",
 	},
 	{
 	 	"code": "isms.codegen.putgithuboptions",
 	 	"name": "代码生成 - Github - 更新设置",
 	},
 	{
 	 	"code": "isms.codegen.putcommonoptions",
 	 	"name": "代码生成 - 基本设置 - 更新",
 	},
 	{
 	 	"code": "isms.codegen.npmoptions",
 	 	"name": "代码生成 - NPM - 设置",
 	},
 	{
 	 	"code": "isms.codegen.githuboptions",
 	 	"name": "代码生成 - Github - 设置",
 	},
 	{
 	 	"code": "isms.codegen.gen",
 	 	"name": "代码生成 - SDK - 预览生成代码",
 	},
 	{
 	 	"code": "isms.codegen.get",
 	 	"name": "代码生成 - 客户端列表",
 	},
 	{
 	 	"code": "isms.codegen.commonoptions",
 	 	"name": "代码生成 - 基本设置 - 获取",
 	},
 	{
 	 	"code": "isms.codegen.all",
 	 	"name": "代码生成 - 所有权限",
 	}
]


标识
----------------------

.. code-block:: javascript

  [
 	{
 	 	"code": "isms.identityresource.put",
 	 	"name": "身份服务 - 更新",
 	},
 	{
 	 	"code": "isms.identityresource.post",
 	 	"name": "身份服务 - 创建",
 	},
 	{
 	 	"code": "isms.identityresource.get",
 	 	"name": "身份服务 - 列表",
 	},
 	{
 	 	"code": "isms.identityresource.detail",
 	 	"name": "身份服务 - 详情",
 	},
 	{
 	 	"code": "isms.identityresource.delete",
 	 	"name": "身份服务 - 删除",
 	},
 	{
 	 	"code": "isms.identityresource.all",
 	 	"name": "身份服务 - 所有权限",
 	}
]


角色
----------------------

.. code-block:: javascript
  
  [
 	{
 	 	"code": "isms.role.all",
 	 	"name": "角色 - 所有权限",
 	},
 	{
 	 	"code": "isms.role.delete",
 	 	"name": "角色 - 删除",
 	},
 	{
 	 	"code": "isms.role.detail",
 	 	"name": "角色 - 详情",
 	},
 	{
 	 	"code": "isms.role.put",
 	 	"name": "角色 - 更新",
 	},
 	{
 	 	"code": "isms.role.post",
 	 	"name": "角色 - 创建",
 	},
 	{
 	 	"code": "isms.role.get",
 	 	"name": "角色 - 列表",
 	}
]



租户
----------------------

.. code-block:: javascript
  
  [
 	{
 	 	"code": "isms.tenant.post",
 	 	"name": "租户 - 创建",
 	},
 	{
 	 	"code": "isms.tenant.get",
 	 	"name": "租户 - 列表",
 	},
 	{
 	 	"code": "isms.tenant.detail",
 	 	"name": "租户 - 详情",
 	},
 	{
 	 	"code": "isms.tenant.delete",
 	 	"name": "租户 - 删除",
 	},
 	{
 	 	"code": "isms.tenant.all",
 	 	"name": "租户 - 所有权限",
 	},
 	{
 	 	"code": "isms.tenant.put",
 	 	"name": "租户 - 更新",
 	}
]


用户
----------------------

.. code-block:: javascript
  
  [
    {
     "code": "isms.user.verifyemail",
     "name": "用户 - 注册 - 发送邮件验证码",
    },
    {
     "code": "isms.user.register",
     "name": "用户 - 注册",
    },
    {
     "code": "isms.user.put",
     "name": "用户 - 更新",
    },
    {
     "code": "isms.user.post",
     "name": "用户 - 创建",
    },
    {
     "code": "isms.user.head",
     "name": "用户 - 是否存在",
    },
    {
     "code": "isms.user.get",
     "name": "用户 - 列表",
    },
    {
     "code": "isms.user.detail",
     "name": "用户 - 详情",
    },
    {
     "code": "isms.user.delete",
     "name": "用户 - 删除",
    },
    {
     "code": "isms.user.verifyphone",
     "name": "用户 - 注册 - 发送手机验证码",
    },
    {
     "code": "isms.user.all",
     "name": "用户 - 所有权限",
    }
]

其他
----------------------


.. code-block:: javascript
  
    {
      "code": "isms.all",
      "name": "授权中心 - 所有权限",
    }