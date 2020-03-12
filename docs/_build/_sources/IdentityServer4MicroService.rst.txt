标识中心
=================
.. Note::

	API(API Resource)、Blob、客户端（Client）、代码生成（CodeGen）、标识资源（Identity Resource）、用户(user)、角色(Role)、租户(Tenant)接口文档。	

- **scope是客户端调用API时需要的权限，permission是用户调用API时需要的权限。**

API
----------------------

列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.apiresource.get
	<br /><br />
	</p>

|

创建
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.apiresource.post
	<br /><br />
	</p>

|

更新
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.apiresource.put
	<br /><br />
	</p>

|

详情
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.apiresource.detail
	<br /><br />
	</p>

|

删除
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.apiresource.delete
	<br /><br />
	</p>

|

导入
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

权限代码
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.apiresource.scopes
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	API代码对照表
	<br /><br />
	</p>

|

网关
~~~~~~~~~~~~~~~~~~~~~~

发布或更新版本
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.publish
	<br /><br />
	</p>

|

创建修订版
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.publishrevision
	<br /><br />
	</p>

|

创建新版本
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.publishversion
	<br /><br />
	</p>

|

上次发布配置
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.publishconfiguration
	<br /><br />
	</p>

|

版本列表
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.versions
	<br /><br />
	</p>

|

上线指定版本
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.setonlineversion
	<br /><br />
	</p>

|

OAuthServers
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.authservers
	<br /><br />
	</p>

|

产品包列表
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.products
	<br /><br />
	</p>

|

修订内容
~~~~~~~~~~~~~~~~~~~~~~

列表
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.releases
	<br /><br />
	</p>

|

发布
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.postrelease
	<br /><br />
	</p>

|

更新
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.putrelease
	<br /><br />
	</p>

|

删除
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.deleterelease
	<br /><br />
	</p>

|

订阅者
~~~~~~~~~~~~~~~~~~~~~~

列表
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.subscriptions
	<br /><br />
	</p>

|

添加
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

取消
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

验证邮箱
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.verifyemail
	<br /><br />
	</p>

|

包市场
~~~~~~~~~~~~~~~~~~~~~~

列表
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.packages
	<br /><br />
	</p>

|

添加
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.postpackages
	<br /><br />
	</p>

|

删除
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.deletepackage
	<br /><br />
	</p>

|

更新
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope&permission：isms.apiresource.deletepackage
	<br /><br />
	</p>

|


Blob
----------------------

File
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	视频支持：avi,quicktime,asf,wmv,flv,matroska,mp4,webm,wmv,flash,mpeg。文档支持：pdf,word,excel。scope：isms.blob.post
	<br /><br />
	</p>

|

上传图片
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	支持图片：jpeg,jpg,png,octet-stream，小于10MB。scope：isms.blob.image
	<br /><br />
	</p>

|

Base64
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	上传Base64格式的png图片。scope：isms.blob.base64
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|


客户端
----------------------

列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.get
	<br /><br />
	</p>

|

创建
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.post
	<br /><br />
	</p>

|

更新
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.put
	<br /><br />
	</p>

|

详情
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.detail
	<br /><br />
	</p>

|

删除
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.delete
	<br /><br />
	</p>

|

创建令牌
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.issuetoken
	<br /><br />
	</p>

|

生成密钥
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.client.postsecretkey
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	客户端代码对照表
	<br /><br />
	</p>

|


代码生成
----------------------

客户端列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope：isms.codegen.clients
	<br /><br />
	</p>

|

服务端列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope：isms.codegen.servers
	<br /><br />
	</p>

|

NPM
~~~~~~~~~~~~~~~~~~~~~~

设置
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.npmoptions
	<br /><br />
	</p>

|

更新设置
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.putnpmoptions
	<br /><br />
	</p>

|

Github
~~~~~~~~~~~~~~~~~~~~~~

设置
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.githuboptions
	<br /><br />
	</p>

|

更新设置
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.putgithuboptions
	<br /><br />
	</p>

|

同步
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.putgithuboptions
	<br /><br />
	</p>

|

基本设置
~~~~~~~~~~~~~~~~~~~~~~

获取
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.commonoptions
	<br /><br />
	</p>

|

更新
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.putcommonoptions
	<br /><br />
	</p>

|

SDK
~~~~~~~~~~~~~~~~~~~~~~

发布
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.codegen.releasesdk
	<br /><br />
	</p>

|

预览生成代码
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

发布记录
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	scope：isms.apiresource.history
	<br /><br />
	</p>

|

添加记录
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|


标识
----------------------

列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.identityresource.get
	<br /><br />
	</p>

|

创建
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.identityresource.post
	<br /><br />
	</p>

|

更新
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.identityresource.put
	<br /><br />
	</p>

|

详情
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.identityresource.detail
	<br /><br />
	</p>

|

删除
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.identityresource.delete
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	标识错误码对照表
	<br /><br />
	</p>

|


角色
----------------------

列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.role.get
	<br /><br />
	</p>

|

创建
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.role.post
	<br /><br />
	</p>

|

更新
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.role.put
	<br /><br />
	</p>

|

详情
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.role.detail
	<br /><br />
	</p>

|

删除
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.role.delete
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	角色码对照表
	<br /><br />
	</p>

|


租户
----------------------

列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.tenant.get
	<br /><br />
	</p>

|

创建
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.tenant.post
	<br /><br />
	</p>

|

更新
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.tenant.put
	<br /><br />
	</p>

|

详情
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.tenant.detail
	<br /><br />
	</p>

|

删除
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.tenant.delete
	<br /><br />
	</p>

|

详情（公共）
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	租户 - 错误码对照表
	<br /><br />
	</p>

|


用户
----------------------

列表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.user.get
	<br /><br />
	</p>

|

创建
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.user.post
	<br /><br />
	</p>

|

更新
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.user.put
	<br /><br />
	</p>

|

详情
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.user.detail
	<br /><br />
	</p>

|

删除
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.user.delete
	<br /><br />
	</p>

|

是否存在
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	scope&permission：isms.user.head
	<br /><br />
	</p>

|

错误码表
~~~~~~~~~~~~~~~~~~~~~~

.. raw:: html

	<p>
	用户错误码对照表
	<br /><br />
	</p>

|

注册
~~~~~~~~~~~~~~~~~~~~~~

提交
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	需验证手机号；邮箱如果填写了，也需要验证
	<br /><br />
	</p>

|

发送手机验证码
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

发送邮件验证码
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|

忘记密码
~~~~~~~~~~~~~~~~~~~~~~

手机验证码
^^^^^^^^^^^^^^^^^^^^^^^^^^^

.. raw:: html

	<p>
	
	<br /><br />
	</p>

|
