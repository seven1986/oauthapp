# 权限码表

!!! note ""
    OAuthApp内置的接口权限，使用Client请求用户授权时，scope参数可参考下面的code。


## 微服务

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.apiresource.post` | 创建 |
| `isms.apiresource.get` | 列表 |
| `isms.apiresource.detail` | 详情 |
| `isms.apiresource.delete` | 删除 |
| `isms.apiresource.put` | 更新 |
| `isms.apiresource.scopes` | 权限代码 |
| `isms.apiresource.publishrevision`  | 网关 - 创建修订版  |
| `isms.apiresource.publishconfiguration` | 网关 - 上次发布配置 |
| `isms.apiresource.publish` | 网关 - 发布或更新版本 |
| `isms.apiresource.products` | 网关 - 产品组 |
| `isms.apiresource.publishversion` | 网关 - 创建新版本 |
| `isms.apiresource.authservers` | 网关 - OAuthServers |
| `isms.apiresource.versions` | 网关 - 版本列表 |
| `isms.apiresource.setonlineversion` | 网关 - 上线指定版本 |
| `isms.apiresource.postrelease` | 修订内容 - 发布 |
| `isms.apiresource.releases` | 修订内容 - 列表 |
| `isms.apiresource.putrelease` | 修订内容 - 更新 |
| `isms.apiresource.deleterelease` | 修订内容 - 删除 |
| `isms.apiresource.postpackages` | 包市场 - 添加 |
| `isms.apiresource.putpackage` | 包市场 - 更新 |
| `isms.apiresource.packages` | 包市场 - 列表 |
| `isms.apiresource.deletepackage` | 包市场 - 删除 |
| `isms.apiresource.verifyemail` | 订阅者 - 验证邮箱 |
| `isms.apiresource.subscriptions` | 订阅者 - 列表 |
| `isms.apiresource.history` | SDK - 发布记录 |
| `isms.apiresource.all` | 所有权限 |

## Blob

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.blob.post` | 上传视频或文档 |
| `isms.blob.image` | 上传图片 |
| `isms.blob.base64` | 传base64格式的png图片 |
| `isms.blob.all` | 所有权限 |

## 客户端

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.client.put` | 更新 |
| `isms.client.postsecretkey` | 生成密钥 |
| `isms.client.post` | 创建 |
| `isms.client.issuetoken` | 创建令牌 |
| `isms.client.get` | 列表 |
| `isms.client.detail` | 详情 |
| `isms.client.delete` | 删除 |
| `isms.client.all` | 所有权限 |


## 代码生成

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.codegen.syncgithub` | Github - 同步 |
| `isms.codegen.servers` | 服务端列表 |
| `isms.codegen.releasesdk` | SDK - 发布 |
| `isms.codegen.putnpmoptions` | NPM - 更新设置 |
| `isms.codegen.putgithuboptions` | Github - 更新设置 |
| `isms.codegen.putcommonoptions` | 基本设置 - 更新 |
| `isms.codegen.npmoptions` | NPM - 设置 |
| `isms.codegen.githuboptions` | Github - 设置 |
| `isms.codegen.gen` | SDK - 预览生成代码 |
| `isms.codegen.get` | 客户端列表 |
| `isms.codegen.commonoptions` | 基本设置 - 获取 |
| `isms.codegen.all` | 所有权限 |


## 标识

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.identityresource.put` | 更新 |
| `isms.identityresource.post` | 创建 |
| `isms.identityresource.get` | 列表 |
| `isms.identityresource.detail` | 详情 |
| `isms.identityresource.delete` | 删除 |
| `isms.identityresource.all` | 所有权限 |


## 角色

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.role.delete` | 删除 |
| `isms.role.detail` | 详情 |
| `isms.role.put` | 更新 |
| `isms.role.post` | 创建 |
| `isms.role.get` | 列表 |
| `isms.role.all` | 所有权限 |


## 租户

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.tenant.post` | 创建 |
| `isms.tenant.get` | 列表 |
| `isms.tenant.detail` | 详情 |
| `isms.tenant.delete` | 删除 |
| `isms.tenant.put` | 更新 |
| `isms.tenant.all` | 所有权限 |


## 用户

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.user.verifyemail` | 注册 - 发送邮件验证码 |
| `isms.user.register` | 注册 |
| `isms.user.put` | 更新 |
| `isms.user.post` | 创建 |
| `isms.user.head` | 是否存在 |
| `isms.user.get` | 列表 |
| `isms.user.detail` | 详情 |
| `isms.user.delete` | 删除 |
| `isms.user.verifyphone` | 注册 - 发送手机验证码 |
| `isms.user.all` | 所有权限 |


## 其他

| code      | name                          |
| ----------- | ------------------------------------ |
| `isms.all` | 所有权限 |