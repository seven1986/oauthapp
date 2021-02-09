# 配置存储服务

!!! note ""
    [Blob接口](https://www.oauthapp.com/docs/index.html#tag/Blob)可存储文件、图片、音视频等，需要开通[Azure Storage 服务](https://azure.microsoft.com/zh-cn/services/storage/blobs/)并复制连接密钥到如下配置中。

=== "appsettings.json"
    ``` json linenums="1"
      "ConnectionStrings": {
        "AzureStorageConnection": "连接字符串"
        }
    ```
