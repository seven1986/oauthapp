# 权限码表

!!! note ""
    OAuthApp内置的接口权限，使用Client请求用户授权时，scope参数可参考下面的code。

### 应用程序

## 标识权限

| code      | name                          |
| ----------- | ------------------------------------ |
| `openid` | 用户的ID |
| `profile` | 用户资料详情 |
| `address` | 联系地址 |
| `email` | 邮箱地址 |
| `phone` | 手机号 |
| `role` | 角色 |
| `permission` | 权限 |
| `offline_access` | 脱机访问 |

## API资源

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.apiresource.post` | 创建 |
| `oauthapp.apiresource.get` | 列表 |
| `oauthapp.apiresource.detail` | 详情 |
| `oauthapp.apiresource.delete` | 删除 |
| `oauthapp.apiresource.put` | 更新 |
| `oauthapp.apiresource.scopes` | 权限代码 |
| `oauthapp.apiresource.publishrevision`  | 网关 - 创建修订版  |
| `oauthapp.apiresource.publishconfiguration` | 网关 - 上次发布配置 |
| `oauthapp.apiresource.publish` | 网关 - 发布或更新版本 |
| `oauthapp.apiresource.products` | 网关 - 产品组 |
| `oauthapp.apiresource.publishversion` | 网关 - 创建新版本 |
| `oauthapp.apiresource.authservers` | 网关 - OAuthServers |
| `oauthapp.apiresource.versions` | 网关 - 版本列表 |
| `oauthapp.apiresource.setonlineversion` | 网关 - 上线指定版本 |
| `oauthapp.apiresource.postrelease` | 修订内容 - 发布 |
| `oauthapp.apiresource.releases` | 修订内容 - 列表 |
| `oauthapp.apiresource.putrelease` | 修订内容 - 更新 |
| `oauthapp.apiresource.deleterelease` | 修订内容 - 删除 |
| `oauthapp.apiresource.postpackages` | 包市场 - 添加 |
| `oauthapp.apiresource.putpackage` | 包市场 - 更新 |
| `oauthapp.apiresource.packages` | 包市场 - 列表 |
| `oauthapp.apiresource.deletepackage` | 包市场 - 删除 |
| `oauthapp.apiresource.verifyemail` | 订阅者 - 验证邮箱 |
| `oauthapp.apiresource.subscriptions` | 订阅者 - 列表 |
| `oauthapp.apiresource.history` | SDK - 发布记录 |
| `oauthapp.apiresource.all` | 所有权限 |

## API权限

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.apiscope.post` | 创建 |
| `oauthapp.apiscope.get` | 列表 |
| `oauthapp.apiscope.detail` | 详情 |
| `oauthapp.apiscope.delete` | 删除 |
| `oauthapp.apiscope.put` | 更新 |

## Blob

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.blob.post` | 上传视频或文档 |
| `oauthapp.blob.image` | 上传图片 |
| `oauthapp.blob.base64` | 传base64格式的png图片 |
| `oauthapp.blob.all` | 所有权限 |

## 客户端

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.client.put` | 更新 |
| `oauthapp.client.postsecretkey` | 生成密钥 |
| `oauthapp.client.post` | 创建 |
| `oauthapp.client.issuetoken` | 创建令牌 |
| `oauthapp.client.get` | 列表 |
| `oauthapp.client.detail` | 详情 |
| `oauthapp.client.delete` | 删除 |
| `oauthapp.client.all` | 所有权限 |


## 软件包

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.package.get` | 软件包 - 列表 |
| `oauthapp.package.detail` | 软件包 - 详情 |
| `oauthapp.package.post` | 软件包 - 创建 |
| `oauthapp.package.put` | 软件包 - 更新 |
| `oauthapp.package.delete` | 软件包 - 删除 |
| `oauthapp.package.releasehistory` | 软件包 - 发布记录 |
| `oauthapp.package.publish` | 软件包 - 发布 |
| `oauthapp.package.precompile` | 软件包 - 预编译 |
| `oauthapp.package.all` | 所有权限 |


## 标识

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.identityresource.put` | 更新 |
| `oauthapp.identityresource.post` | 创建 |
| `oauthapp.identityresource.get` | 列表 |
| `oauthapp.identityresource.detail` | 详情 |
| `oauthapp.identityresource.delete` | 删除 |
| `oauthapp.identityresource.all` | 所有权限 |


## 角色

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.role.delete` | 删除 |
| `oauthapp.role.detail` | 详情 |
| `oauthapp.role.put` | 更新 |
| `oauthapp.role.post` | 创建 |
| `oauthapp.role.get` | 列表 |
| `oauthapp.role.all` | 所有权限 |


## 租户

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.tenant.post` | 创建 |
| `oauthapp.tenant.get` | 列表 |
| `oauthapp.tenant.detail` | 详情 |
| `oauthapp.tenant.delete` | 删除 |
| `oauthapp.tenant.put` | 更新 |
| `oauthapp.tenant.all` | 所有权限 |


## 用户

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.user.verifyemail` | 注册 - 发送邮件验证码 |
| `oauthapp.user.register` | 注册 |
| `oauthapp.user.put` | 更新 |
| `oauthapp.user.post` | 创建 |
| `oauthapp.user.head` | 是否存在 |
| `oauthapp.user.get` | 列表 |
| `oauthapp.user.detail` | 详情 |
| `oauthapp.user.delete` | 删除 |
| `oauthapp.user.verifyphone` | 注册 - 发送手机验证码 |
| `oauthapp.user.all` | 所有权限 |


## 其他

| code      | name                          |
| ----------- | ------------------------------------ |
| `oauthapp.all` | 所有权限 |