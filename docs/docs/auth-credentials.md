# 客户端模式

!!! note ""
    客户端模式（client credentials）一般用于资源服务器是应用的一个后端模块，客户端向认证服务器验证身份来获取令牌。

## 适用场景

没有用户参与的，完全信任的服务器端服务。

## 使用说明

### 获取AccessToken

!!! note ""
    当使用Client发起一个客户端模式请求时，标准的请求地址是：**--POST {server}/connect/token** 

#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `grant_type` | 是 | 授权类型，值固定为`client_credentials` |
| `client_id` | 是 | 应用的ID |
| `client_secret` | 是 | 应用的密钥 |
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