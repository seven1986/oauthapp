# 混合模式

!!! note ""
    混合模式（hybrid）是简化模式、授权码模式两者的混合，在该流程里，有一些Tokens和授权码来自于授权重点，而另外一些Tokens则来自于Token端点。该流程允许客户端立即使用ID Token，并且只需要一次往返即可获得授权码。

## 适用场景

客户端应用可以安全的维护Secret，它也适合于长时间的访问。


## 使用说明

### 1，获取授权码和标识令牌

!!! note ""
    当使用Client发起一个授权码模式请求时，标准的请求地址是：**--GET {server}/connect/authorize?response_type=id_token code&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&nonce={nonce}** 


#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `response_type` | 是 | 固定传id_token code |
| `client_id` | 是 | 应用的ID |
| `redirect_uri` | 是 | 回调地址 |
| `scope` | 是 | 申请授权的[scope](/scopes)，多个scope用空格分隔。 |
| `nonce` | 是 | 随机字符串 |


#### 响应格式

!!! note ""
    当用户授权成功后，服务器会将授权码和身份令牌一起回传到回调地址：**{redirect_uri}?code=`***code***`&id_token=`***id_token***`**


### 2，获取AccessToken

!!! note ""
    通过code（授权码）换取AccessToken，标准的请求地址是：**--POST {server}/connect/token** 

#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `grant_type` | 是 | 授权类型，可选值为`code id_token`、`code token`、`code id_token token` |
| `code` | 是 | 步骤1返回的授权码 |
| `client_id` | 是 | 应用的ID |
| `client_secret` | 是 | 应用的密钥 |
| `scope` | 是 | 表示授权范围，多个[scope](/scopes)用空格分隔。 |
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

### 3，获取用户信息

!!! note ""
    通过id_token（身份令牌）获取用户信息，标准的请求地址是：**--POST {server}/connect/userinfo  --header Authorization: Bearer`***id_token***`** 


#### 响应格式
```json
    {
        "permission": "***permission***",
        "role": "***role***",
        "preferred_username": "***preferred_username***",
        "name": "***name***",
        "sub": "1"
    }
```