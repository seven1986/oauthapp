# 密码模式

!!! note ""
    密码模式（password）使用用户名/密码作为授权方式从授权服务器上获取令牌，一般不支持刷新令牌。

## 适用场景

第一方单页应用与第一方原生App。

## 使用说明

### 获取AccessToken

!!! note ""
    当使用Client发起一个密码模式请求时，标准的请求地址是：**--POST {server}/connect/token** 

#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `grant_type` | 是 | 授权类型，值固定为`password` |
| `client_id` | 是 | 应用的ID |
| `client_secret` | 是 | 应用的密钥 |
| `username` | 是 | 用户的登录账号 |
| `password` | 是 | 用户的登录密码 |
| `scope` | 是 | 表示授权范围，多个[scope](/scopes)用空格分隔。 |


#### 响应格式
```json
    {
        "access_token":"***AccessToken***",
        "expires_in":3600,
        "token_type":"Bearer",
        "scope":"***scopes***"
    }
```