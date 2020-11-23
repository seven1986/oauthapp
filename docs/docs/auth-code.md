# 授权码模式

!!! note ""
    授权码模式（authorization code）是功能最完整、流程最严密的授权模式，code保证了token的安全性，即使code被拦截，由于没有app_secret，也是无法通过code获得token的。

## 适用场景

第三方Web服务器端应用与第三方原生App。

## 使用说明

### 1，获取授权码

!!! note ""
    当使用Client发起一个授权码模式请求时，标准的请求地址是：**--GET {server}/connect/authorize?response_type=code&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&nonce={nonce}** 


#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `response_type` | 是 | 响应类型，值固定为`code` |
| `client_id` | 是 | 应用的ID |
| `redirect_uri` | 是 | 回调地址 |
| `scope` | 是 | 申请授权的[scope](/scopes)，多个scope用空格分隔。 |
| `nonce` | 是 | 随机字符串 |


#### 响应格式

!!! note ""
    当用户授权成功后，服务器会将授权码一起回传到回调地址：**{redirect_uri}?code={code}**


### 2，获取AccessToken

!!! note ""
    通过code（授权码）换取AccessToken，标准的请求地址是：**--POST {server}/connect/token** 

#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `grant_type` | 是 | 授权类型，值固定为`authorization_code` |
| `code` | 是 | 步骤1返回的授权码 |
| `client_id` | 是 | 应用的ID |
| `client_secret` | 是 | 应用的密钥 |
| `scope` | 是 | 申请授权的[scope](/scopes)，多个scope用空格分隔。 |
| `redirect_uri` | 是 | 与步骤1的回调地址保持一致 |

#### 响应格式
```json
    {
        "access_token":"***AccessToken***",
        "expires_in":3600,
        "token_type":"Bearer",
        "scope":"***scopes***"
    }
```