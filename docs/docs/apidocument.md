# 接口文档


https://www.oauthapp.com/swagger/v1/swagger.json
## ApiResource


### API - 列表

!!! note "" 
	scope&permission：oauthapp.apiresource.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `q.name` | query | 名称 |
| `q.expandScopes` | query | 是否展开所有Scope（默认为false） |
| `q.expandClaims` | query | 是否展开所有Claims（默认为false） |
| `orderby` | query | 排序字段 |
| `asc` | query | 正序或倒序 |
| `skip` | query | 跳过的数据条数 |
| `take` | query | 将获取的数据条数 |
| `startTime` | query | 开始时间。yyyy-MM-dd |
| `endTime` | query | 结束时间。yyyy-MM-dd |
| `api-version` | query |  |


### API - 创建

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 更新

!!! note "" 
	scope&permission：oauthapp.apiresource.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 详情

!!! note "" 
	scope&permission：oauthapp.apiresource.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### API - 删除

!!! note "" 
	scope&permission：oauthapp.apiresource.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | ID |
| `api-version` | query |  |


### API - 导入

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 错误码表

!!! note "" 
	API代码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 网关 - 发布或更新版本

!!! note "" 
	scope&permission：oauthapp.apiresource.publish

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 网关 - 创建修订版

!!! note "" 
	scope&permission：oauthapp.apiresource.publishrevision

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 网关 - 创建新版本

!!! note "" 
	scope&permission：oauthapp.apiresource.publishversion

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 网关 - 上次发布配置

!!! note "" 
	scope&permission：oauthapp.apiresource.publishconfiguration

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 网关 - 版本列表

!!! note "" 
	scope&permission：oauthapp.apiresource.versions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 网关 - 上线指定版本

!!! note "" 
	scope&permission：oauthapp.apiresource.setonlineversion

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `revisionId` | path |  |
| `api-version` | query |  |


### API - 网关 - OAuthServers

!!! note "" 
	scope&permission：oauthapp.apiresource.authservers

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 网关 - 产品包列表

!!! note "" 
	scope&permission：oauthapp.apiresource.products

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 修订内容 - 列表

!!! note "" 
	scope&permission：oauthapp.apiresource.releases

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `apiId` | query | Api的ID |
| `api-version` | query |  |


### API - 修订内容 - 发布

!!! note "" 
	scope&permission：oauthapp.apiresource.postrelease

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 修订内容 - 更新

!!! note "" 
	scope&permission：oauthapp.apiresource.putrelease

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `releaseId` | path | 修订内容的ID |
| `api-version` | query |  |


### API - 修订内容 - 删除

!!! note "" 
	scope&permission：oauthapp.apiresource.deleterelease

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `releaseId` | path | 修订内容的ID |
| `api-version` | query |  |


### API - 订阅者 - 列表

!!! note "" 
	scope&permission：oauthapp.apiresource.subscriptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 订阅者 - 添加

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `code` | query | 邮箱校验加密字符串 |
| `api-version` | query |  |


### API - 订阅者 - 取消

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `code` | query | 邮箱校验加密字符串 |
| `api-version` | query |  |


### API - 订阅者 - 验证邮箱

!!! note "" 
	scope&permission：oauthapp.apiresource.verifyemail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 包市场 - 列表

!!! note "" 
	scope&permission：oauthapp.apiresource.packages

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 包市场 - 添加

!!! note "" 
	scope&permission：oauthapp.apiresource.postpackages

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### API - 包市场 - 删除

!!! note "" 
	scope&permission：oauthapp.apiresource.deletepackage

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `packageId` | path | 包的ID |
| `api-version` | query |  |


### API - 包市场 - 更新

!!! note "" 
	scope&permission：oauthapp.apiresource.deletepackage

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `packageId` | path | 包的ID |
| `api-version` | query |  |


## ApiScope


### API - 列表

!!! note "" 
	scope&permission：isms.apiscope.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `q.name` | query | 名称 |
| `q.expandProperties` | query | 是否展开所有Properties（默认为false） |
| `q.expandClaims` | query | 是否展开所有Claims（默认为false） |
| `orderby` | query | 排序字段 |
| `asc` | query | 正序或倒序 |
| `skip` | query | 跳过的数据条数 |
| `take` | query | 将获取的数据条数 |
| `startTime` | query | 开始时间。yyyy-MM-dd |
| `endTime` | query | 结束时间。yyyy-MM-dd |
| `api-version` | query |  |


### API - 创建

!!! note "" 
	scope&permission：isms.apiscope.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 更新

!!! note "" 
	scope&permission：isms.apiscope.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### API - 详情

!!! note "" 
	scope&permission：isms.apiscope.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### API - 删除

!!! note "" 
	scope&permission：isms.apiscope.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | ID |
| `api-version` | query |  |


### API - 错误码表

!!! note "" 
	API代码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


## Blob


### Blob - File

!!! note "" 
	视频支持：avi,quicktime,asf,wmv,flv,matroska,mp4,webm,wmv,flash,mpeg。文档支持：pdf,word,excel。scope：isms.blob.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `folderName` | query | 文件夹名称,5~30个字节，英文或数字组装成。默认为当前日期yyyyMMdd |
| `api-version` | query |  |


### Blob - 上传图片

!!! note "" 
	支持图片：jpeg,jpg,png,octet-stream，小于10MB。scope：isms.blob.image

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `folderName` | query | 文件夹名称,5~30个字节，英文或数字组装成。默认为当前日期yyyyMMdd |
| `api-version` | query |  |


### Blob - Base64

!!! note "" 
	上传Base64格式的png图片。scope：isms.blob.base64

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `folderName` | query | 文件夹名称,5~30个字节，英文或数字组装成。默认为当前日期yyyyMMdd |
| `api-version` | query |  |


### Blob - 错误码表

!!! note "" 
	Blob错误码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


## Client


### 客户端 - 列表

!!! note "" 
	scope&permission：isms.client.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `q.ClientID` | query |  |
| `q.ClientName` | query |  |
| `orderby` | query | 排序字段 |
| `asc` | query | 正序或倒序 |
| `skip` | query | 跳过的数据条数 |
| `take` | query | 将获取的数据条数 |
| `startTime` | query | 开始时间。yyyy-MM-dd |
| `endTime` | query | 结束时间。yyyy-MM-dd |
| `api-version` | query |  |


### 客户端 - 创建

!!! note "" 
	scope&permission：isms.client.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 客户端 - 更新

!!! note "" 
	scope&permission：isms.client.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 客户端 - 详情

!!! note "" 
	scope&permission：isms.client.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 客户端 - 删除

!!! note "" 
	scope&permission：isms.client.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 客户端 - 创建令牌

!!! note "" 
	scope&permission：isms.client.issuetoken

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 客户端 - 生成密钥

!!! note "" 
	scope&permission：isms.client.postsecretkey

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 客户端 - 错误码表

!!! note "" 
	客户端代码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


## CodeGen


### 代码生成 - 客户端列表

!!! note "" 
	scope：isms.codegen.clients

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `fromCache` | query |  |
| `api-version` | query |  |


### 代码生成 - 服务端列表

!!! note "" 
	scope：isms.codegen.servers

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `fromCache` | query |  |
| `api-version` | query |  |


### 代码生成 - NPM - 设置

!!! note "" 
	scope：isms.codegen.npmoptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `language` | path | 语言 |
| `api-version` | query |  |


### 代码生成 - NPM - 更新设置

!!! note "" 
	scope：isms.codegen.putnpmoptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `language` | path | 语言 |
| `api-version` | query |  |


### 代码生成 - Github - 设置

!!! note "" 
	scope：isms.codegen.githuboptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### 代码生成 - Github - 更新设置

!!! note "" 
	scope：isms.codegen.putgithuboptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### 代码生成 - Github - 同步

!!! note "" 
	scope：isms.codegen.putgithuboptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### 代码生成 - 基本设置 - 获取

!!! note "" 
	scope：isms.codegen.commonoptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### 代码生成 - 基本设置 - 更新

!!! note "" 
	scope：isms.codegen.putcommonoptions

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### 代码生成 - SDK - 发布

!!! note "" 
	scope：isms.codegen.releasesdk

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 代码生成 - SDK - 预览生成代码

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 代码生成 - SDK - 发布记录

!!! note "" 
	scope：isms.apiresource.history

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


### 代码生成 - SDK - 添加记录

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path | API的ID |
| `api-version` | query |  |


## IdentityResource


### 标识 - 列表

!!! note "" 
	scope&permission：isms.identityresource.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `q.Name` | query |  |
| `orderby` | query | 排序字段 |
| `asc` | query | 正序或倒序 |
| `skip` | query | 跳过的数据条数 |
| `take` | query | 将获取的数据条数 |
| `startTime` | query | 开始时间。yyyy-MM-dd |
| `endTime` | query | 结束时间。yyyy-MM-dd |
| `api-version` | query |  |


### 标识 - 创建

!!! note "" 
	scope&permission：isms.identityresource.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 标识 - 更新

!!! note "" 
	scope&permission：isms.identityresource.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 标识 - 详情

!!! note "" 
	scope&permission：isms.identityresource.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 标识 - 删除

!!! note "" 
	scope&permission：isms.identityresource.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 标识 - 错误码表

!!! note "" 
	标识错误码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


## Role


### 角色 - 列表

!!! note "" 
	scope&permission：isms.role.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 角色 - 创建

!!! note "" 
	scope&permission：isms.role.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 角色 - 更新

!!! note "" 
	scope&permission：isms.role.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 角色 - 详情

!!! note "" 
	scope&permission：isms.role.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 角色 - 删除

!!! note "" 
	scope&permission：isms.role.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 角色 - 错误码表

!!! note "" 
	角色码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


## Tenant


### 租户 - 列表

!!! note "" 
	scope&permission：oauthapp.tenant.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `q.Host` | query | 站点域名 |
| `orderby` | query | 排序字段 |
| `asc` | query | 正序或倒序 |
| `skip` | query | 跳过的数据条数 |
| `take` | query | 将获取的数据条数 |
| `startTime` | query | 开始时间。yyyy-MM-dd |
| `endTime` | query | 结束时间。yyyy-MM-dd |
| `api-version` | query |  |


### 租户 - 创建

!!! note "" 
	scope&permission：oauthapp.tenant.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 租户 - 更新

!!! note "" 
	scope&permission：oauthapp.tenant.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 租户 - 详情

!!! note "" 
	scope&permission：oauthapp.tenant.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 租户 - 删除

!!! note "" 
	scope&permission：oauthapp.tenant.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 租户 - 详情（公共）

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `host` | query |  |
| `api-version` | query |  |


### 租户 - 错误码表

!!! note "" 
	租户 - 错误码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


## User


### 用户 - 列表

!!! note "" 
	scope&permission：isms.user.get

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `q.role` | query | 用户角色
可选值：user/partner/developer/administrator |
| `q.phoneNumber` | query | 手机号 |
| `q.name` | query | 昵称 |
| `q.email` | query | 邮箱 |
| `q.providerName` | query | 第三方登录平台名称 |
| `q.providerKey` | query | 第三方登陆平台的UserID |
| `q.claimType` | query | claimType |
| `q.claimValue` | query | claimValue |
| `orderby` | query | 排序字段 |
| `asc` | query | 正序或倒序 |
| `skip` | query | 跳过的数据条数 |
| `take` | query | 将获取的数据条数 |
| `startTime` | query | 开始时间。yyyy-MM-dd |
| `endTime` | query | 结束时间。yyyy-MM-dd |
| `api-version` | query |  |


### 用户 - 创建

!!! note "" 
	scope&permission：isms.user.post

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 用户 - 更新

!!! note "" 
	scope&permission：isms.user.put

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 获取团队列表

!!! note "" 
	scope&permission：isms.user.distributors

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `path` | query |  |
| `api-version` | query |  |


### 用户 - 详情

!!! note "" 
	scope&permission：isms.user.detail

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 用户 - 删除

!!! note "" 
	scope&permission：isms.user.delete

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `id` | path |  |
| `api-version` | query |  |


### 用户 - 是否存在

!!! note "" 
	scope&permission：isms.user.head

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 用户 - 错误码表

!!! note "" 
	用户错误码对照表

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 用户 - 注册 - 提交

!!! note "" 
	需验证手机号；邮箱如果填写了，也需要验证

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 用户 - 注册 - 发送手机验证码

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 用户 - 注册 - 发送邮件验证码

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |


### 用户 - 忘记密码 - 手机验证码

!!! note "" 
	

| 名称 | 类型 | 说明 |
| ----------- | ----------- | ----------- |
| `api-version` | query |  |