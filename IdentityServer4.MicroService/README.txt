【★★★☆☆IdentityServer4.MicroService☆☆★★★】
============================================================
【appsettings.json 配置】
- AzureStorageConnection、RedisConnection非必需配置
- SMS和Email可空，或对接对接SendCloud
- Host必须与当前项目访问地址一致，结尾无需带/

	{
	  "ConnectionStrings": {
	    "DataBaseConnection": "Data Source=(localdb)\\ProjectsV13;Initial Catalog=IdentityServer4.MicroService;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False",
	    "AzureStorageConnection": "",
	    "RedisConnection": ""
	  },
	  "IdentityServer": {
	    "Host": "https://localhost:44301",
	    "SMS": {
	      "apiUser": "", 
	      "apiKey": ""
	    },
	    "Email": {
	      "apiUser": "",
	      "apiKey": "",
	      "fromEmail": "",
	      "fromName": ""
	    }
	  }
	}

【启用AspNetCore Identity UI】
https://github.com/seven1986/IdentityServer4.MicroService/wiki/使用示例（基础）

【问题反馈】
https://github.com/seven1986/IdentityServer4.MicroService/issues