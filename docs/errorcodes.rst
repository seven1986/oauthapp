错误码表
========

全局
-------

::

    [{
      "code": 200,
      "name": "Status200OK",
      "description": "ok"
    },
    {
      "code": 422,
      "name": "UnprocessableEntity",
      "description": "请求实体错误"
    },{
      "code": 417,
      "name": "ExpectationFailed",
      "description": "服务器内部错误"
    },
    {
      "code": 404,
      "name": "NotFound",
      "description": "未找到内容"
    }]


微服务
-------

::

    [{
      "code": 300001,
      "name": "Versions_IdCanNotBeNull",
      "description": "请填写Id"
    }, {
      "code": 300002,
      "name": "Versions_GetDetailFailed",
      "description": "获取Api详情失败"
    }, {
      "code": 300003,
      "name": "Versions_GetVersionListFailed",
      "description": "获取Api版本失败"
    }, {
      "code": 300004,
      "name": "PublishRevision_GetDetailFailed",
      "description": "获取Api详情失败"
    }, {
      "code": 300005,
      "name": "PublishRevision_PublishFailed",
      "description": "发布修订版失败"
    }, {
      "code": 300006,
      "name": "PublishRevision_CreateReleaseFailed",
      "description": "更新修订内容失败"
    }, {
      "code": 300007,
      "name": "Releases_IdCanNotBeNull",
      "description": "请填写Id"
    }, {
      "code": 300008,
      "name": "Releases_GetVersionListFailed",
      "description": "获取修订记录列表失败"
    }, {
      "code": 300009,
      "name": "Subscription_PostFailed",
      "description": "添加订阅失败"
    }, {
      "code": 300010,
      "name": "Subscription_VerfifyCodeFailed",
      "description": "邮箱验证码错误"
    }, {
      "code": 300010,
      "name": "Subscription_VerifyEmailFailed",
      "description": "发送邮箱验证码失败"
    }, {
      "code": 300010,
      "name": "SetOnlineVersion_PostFailed",
      "description": "上线版本失败"
    }, {
      "code": 300011,
      "name": "Publish_PublishFailed",
      "description": "发布失败"
    }, {
      "code": 300012,
      "name": "Subscription_DelSubscriptionFailed",
      "description": "取消订阅失败"
    }, {
      "code": 300013,
      "name": "VerifyEmail_AddEmailFailed",
      "description": "邮箱已存在订阅列表"
    }, {
      "code": 300014,
      "name": "Packages_PostFailed",
      "description": "添加包失败"
    }, {
      "code": 300015,
      "name": "Packages_DelPackageFailed",
      "description": "删除包失败"
    }]

|
   
用户
-------

::

    [{
      "code": 100001,
      "name": "Register_PhoneNumberExists",
      "description": "手机号已被注册"
    }, {
      "code": 100002,
      "name": "Register_PhoneNumberVerifyCodeError",
      "description": "无效的手机验证码"
    }, {
      "code": 100003,
      "name": "Register_EmailExists",
      "description": "手机号已被注册"
    }, {
      "code": 100004,
      "name": "Register_EmailVerifyCodeError",
      "description": "手机号已被注册"
    }, {
      "code": 100005,
      "name": "VerifyPhone_CallLimited",
      "description": "该号码已达24小时发送上限"
    }, {
      "code": 100006,
      "name": "VerifyPhone_TooManyRequests",
      "description": "发送过于频繁,请{0}秒后重试"
    }, {
      "code": 100007,
      "name": "VerifyEmail_CallLimited",
      "description": "该号码已达24小时发送上限"
    }, {
      "code": 100008,
      "name": "VerifyEmail_TooManyRequests",
      "description": "发送过于频繁,请{0}秒后重试"
    }, {
      "code": 100009,
      "name": "Post_CreateUserFail",
      "description": "创建用户失败"
    }]
    