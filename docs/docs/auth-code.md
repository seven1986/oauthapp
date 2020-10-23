# 授权码模式

!!! note ""
    授权码模式（authorization code）是功能最完整、流程最严密的授权模式，code保证了token的安全性，即使code被拦截，由于没有app_secret，也是无法通过code获得token的。

## 适用场景

第三方Web服务器端应用与第三方原生App。