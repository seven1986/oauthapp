创建身份认证中心
==================

.. Note::

	参考如下步骤，搭建身份认证中心服务器。


1，创建 AspNet Core Web 应用程序
------------------------------

.. image:: ./images/usecase-basic/startserver1.png
.. image:: ./images/usecase-basic/startserver2.png
.. image:: ./images/usecase-basic/startserver3.png
.. image:: ./images/usecase-basic/startserver4.png

2，安装 IdentityServer4.MicroService
---------------

.. image:: ./images/usecase-basic/startserver5.png
.. image:: ./images/usecase-basic/startserver6.png


3，配置 appsetting.json
----------------------------

- 对项目点击右键——属性——调试，复制项目网址
.. image:: ./images/usecase-basic/startserver7.png

- 参考如下配置，复制到appsetting.json，注意将Host节点替换为当前项目网址，**结尾不要带“/”**
- DataBaseConnection节点为本地测试数据库，可以替换为实际数据库的链接地址
- SMS和Email节点为sendcloud的服务，不使用可以为空
.. code-block:: javascript
  
  "ConnectionStrings": {
    "DataBaseConnection": "Data Source=(localdb)\\ProjectsV13;Initial Catalog=ismsdb_demo;Integrated Security=True;Pooling=False",
    "AzureStorageConnection": "可空"
  	},
  "IdentityServer": {
    "Host": "当前项目网址，结尾不要到“/”",
    "SMS": {
      "apiUser": "可空",
      "apiKey": "可空"
    },
    "Email": {
      "apiUser": "可空",
      "apiKey": "可空",
      "fromEmail": "可空",
      "fromName": "可空"
    }
  }

|
.. image:: ./images/usecase-basic/startserver8.png


4，添加并使用服务
----------------------------

- 打开项目的Views/Shared/_Layout.cshtml文件，添加登陆组件，启用Identity UI。
 
.. image:: ./images/usecase-basic/startserver9.png

- 打开Startup.cs文件，加入下图中的代码。 **系统默认的用户名：admin@admin.com，密码：123456aA!**

.. image:: ./images/usecase-basic/startserver10.png


5，配置成功
--------------

.. image:: ./images/usecase-basic/startserver11.png
.. image:: ./images/usecase-basic/startserver12.png
.. image:: ./images/usecase-basic/startserver13.png
.. image:: ./images/usecase-basic/startserver14.png


6，Swagger UI
--------------

- 在浏览器中打开 **{项目网址}**/swagger/index.html可访问API文档。
- **admin@admin.com默认包含所有接口权限。**

.. image:: ./images/usecase-basic/startserver15.png
.. image:: ./images/usecase-basic/startserver16.png
.. image:: ./images/usecase-basic/startserver17.png
.. image:: ./images/usecase-basic/startserver18.png

- 可访问 **{项目网址}**/grant，撤销对指定client的授权。

.. image:: ./images/usecase-basic/startserver19.png

7，Postman
--------------

- Postman下载地址：https://www.postman.com/downloads/

.. image:: ./images/usecase-basic/startserver20.png
.. image:: ./images/usecase-basic/startserver21.png
.. image:: ./images/usecase-basic/startserver22.png
.. image:: ./images/usecase-basic/startserver23.png
.. image:: ./images/usecase-basic/startserver24.png
