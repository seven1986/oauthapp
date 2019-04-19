创建微服务
===========

.. Note::

	参考如下步骤，创建微服务。


1，创建AspNetCore Web应用程序
------------------------------

.. image:: ./images/usercase-apiresource/1.png
.. image:: ./images/usercase-apiresource/2.png


2，安装Nuget包
---------------

.. image:: ./images/usercase-apiresource/3.png

3，appsetting.json配置
----------------------------

 - 修改项目启动地址为https地址
 - 配置appsetting.json，Host节点为当前项目的https地址
.. image:: ./images/usercase-apiresource/4.png
.. image:: ./images/usercase-apiresource/5.png
.. image:: ./images/usercase-apiresource/6.png

4，Startup.cs添加并使用服务
----------------------------

.. image:: ./images/usercase-apiresource/7.png


5，启动并预览
--------------


.. image:: ./images/usercase-apiresource/8.png
.. image:: ./images/usercase-apiresource/9.png


6，自定义权限
--------------

验证应用权限、用户权限、用户角色等。

.. image:: ./images/usercase-apiresource/10.png
.. image:: ./images/usercase-apiresource/11.png
