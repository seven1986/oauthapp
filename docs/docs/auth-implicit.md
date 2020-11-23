# 简化模式

!!! note ""
    隐式授权模式/简化模式（implicit）和授权码模式类似，只不过少了获取code的步骤，是直接获取令牌token的，适用于公开的浏览器单页应用，令牌直接从授权服务器返回，不支持刷新令牌，且没有code安全保证，令牌容易因为被拦截窃听而泄露。

## 适用场景

第三方单页面应用。

## 使用说明

### 获取AccessToken

!!! note ""
    当使用Client发起一个简化授权请求时，标准的请求地址是：**--GET {server}/connect/authorize?response_type=token&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&nonce={nonce}** 

#### 参数说明

| 名称 | 是否必须 | 说明 |
| ----------- | ----------- | ----------- |
| `response_type` | 是 | 授权类型，可选值为`id_token token` |
| `client_id` | 是 | 应用的ID |
| `redirect_uri` | 是 | 回调地址 |
| `scope` | 是 | 表示授权范围，多个[scope](/scopes)用空格分隔。 |
| `nonce` | 是 | 随机字符串 |


#### 响应格式

!!! note ""
    当用户授权成功后，服务器会将access_token相关信息一起回传到回调地址：**{redirect_uri}?access_token=`***access_token***`&token_type=bearer&expires_in=3600&scope=`***scopes***`**
