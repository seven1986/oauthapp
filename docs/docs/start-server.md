# 安装

!!! note ""
    使用OAuthApp可以快速建立OAuth2.0的Saas用户中心。参考如下步骤创建项目并确保已安装[Visual Studio 2019](https://visualstudio.microsoft.com/zh-hans/vs/)、[DotNet Core 3.1+](https://dotnet.microsoft.com/download/dotnet-core/3.1)、[SQL Server](https://www.microsoft.com/zh-cn/sql-server/sql-server-downloads)。

## 1，创建应用程序

![1](./images/usecase-basic/startserver1.png)
![2](./images/usecase-basic/startserver2.png)
![3](./images/usecase-basic/startserver3.png)
![4](./images/usecase-basic/startserver4.png)

## 2，安装Nuget包

!!! note ""
    对项目的依赖项点右键，选择管理Nuget程序包

![5](./images/usecase-basic/startserver5.png)

!!! note ""
    输入OAuthApp，然后点击安装

![6](./images/usecase-basic/startserver6.jpg)


## 3，配置


### 3.1 appsetting.json

!!! note ""
    对项目依次点击鼠标右键——属性——调试，复制项目网址.

![7](./images/usecase-basic/startserver7.png)

!!! note ""
    复制下面代码到 **appsetting.json** 文件，注意将Host节点替换为当前项目网址，**结尾不要带“/”** 。

    **DataBaseConnection** 可替换为实际数据库的地址，**SMS** 和 **Email** 使用的是[SendCloud](https://sendcloud.sohu.com/)的服务。

=== "appsetting.json"
``` json linenums="1"
  "ConnectionStrings": {
    "DataBaseConnection": "Data Source=(localdb)\\ProjectsV13;Initial Catalog=ismsdb_demo;Integrated Security=True;Pooling=False",
    "AzureStorageConnection": "可空"
  	},
  "IdentityServer": {
    "Host": "当前项目网址，结尾不要到/",
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
```

![8](./images/usecase-basic/startserver8.png)


### 3.2 Identity UI

!!! note ""
    打开 **Startup.cs** 文件，添加如下代码。
    启动项目后可打开测试：{{网站地址}}/tenant/index.html


=== "Startup.cs"
    ``` csharp linenums="1"
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddOAuthApp();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
         if (env.IsDevelopment())
         {
             app.UseDeveloperExceptionPage();
         }
         else
         {
             app.UseExceptionHandler("/Home/Error");
             // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
             app.UseHsts();
         }

         app.UseHttpsRedirection();

        //必须在app.UseHttpsRedirection之后、app.UseStaticFiles之前
         app.UseOAuthAppUI(); 

         app.UseStaticFiles();
    }
    ```


![10](./images/usecase-basic/startserver10.jpg)


## 4，    启动

!!! note ""
	启动项目并预览。默认用户账号：admin@admin.com，密码：123456aA!，拥有所有权限。

![11](./images/usecase-basic/startserver11.png)
![12](./images/usecase-basic/startserver12.png)
![13](./images/usecase-basic/startserver13.png)
![14](./images/usecase-basic/startserver14.png)


## 5，   Swagger UI

!!! note ""
    在浏览器中打开 **{项目网址}** /swagger/index.html可访问API文档。

![15](./images/usecase-basic/startserver15.png)
![16](./images/usecase-basic/startserver16.png)
![17](./images/usecase-basic/startserver17.png)
![18](./images/usecase-basic/startserver18.png)

## 6，Postman

使用 [Postman](https://www.postman.com/downloads){: .md-button } 测试现有接口。

![20](./images/usecase-basic/startserver20.png)
![21](./images/usecase-basic/startserver21.png)
![22](./images/usecase-basic/startserver22.png)
![23](./images/usecase-basic/startserver23.png)
![24](./images/usecase-basic/startserver24.png)
