;
(window => {

    window.AppSettings = {
        Server: "https://www.oauthapp.com",
        Platforms: {
            web: {
                id: 'web',
            },
            wechat: {
                id: 'wechat',
                cookie_user: "_app_user_wechat",
            },
            weibo: {
                id: 'weibo',
                cookie_user: "_app_user_weibo",
            },
            qq: {
                id: 'qq',
                cookie_user: "_app_user_qq",
            }
        }
    };

    AppSettings = {
        ...AppSettings,
        ...document.scripts.namedItem("appcore").dataset
    };

    // query参数
    function queryString(name) {
        try {
            var queryParams = location.search.split("?")[1].split(/&/g);
            queryParams = queryParams.filter(x => x.split('=')[0] == name)[0].split('=')[1];
            return decodeURIComponent(queryParams);
        } catch (err) {
            return null;
        }
    }

    // 默认授权 - 获取用户信息
    function UnionIDSignIn () {

        var promise = new Promise((resolve, reject) => {

            Fingerprint2.get(async components => {

                var values = components.map(x => x.value)

                var unionID = Fingerprint2.x64hash128(values.join(''), 31);

                window.AppUser = {
                    platform: AppSettings.Platforms.web,
                    nickName: unionID,
                    avatar: App.info.logo,
                    unionID: unionID
                };

                await getProfile();

                resolve(window.AppUser);
            });
        });

        return promise;
    }

    // 提交用户数据
    window.putProfile = (_data) => {
        if (_data) {
            AppUser.data = _data;
        }

        var accessToken = Cookies.get('access_token');

        return $.ajax({
            url: `${AppSettings.Server}/api/AppUsers/Profile`,
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${accessToken}`
            },
            data: JSON.stringify({
                avatar: AppUser.avatar || '',
                nickName: AppUser.nickName,
                data: JSON.stringify(AppUser.data) || '{}',
            })
        });
    }

    // 获取用户数据
    async function getProfile() {
        window.access_token = Cookies.get('access_token');

        if (!window.access_token)
        {
            var signInResult = await signIn();
            var signInResultJson = await signInResult.json();
            if (!signInResult.ok || signInResultJson.code!=200) {
                var signUpResult = await signUp();

                if (signUpResult.ok) {
                    var json = await signUpResult.json();

                    if (json.code == 200) {
                        window.access_token = json.data.access_token;
                        Cookies.set('access_token', window.access_token, {
                            expires: json.data.expires_in
                        });
                    }
                }
            } else {
                if (signInResultJson.code == 200) {
                    window.access_token = signInResultJson.data.access_token;
                    Cookies.set('access_token', window.access_token, {
                        expires: signInResultJson.data.expires_in
                    });
                }
            }
        }

        var profileResult = await fetch(`${AppSettings.Server}/api/appUsers/Profile`, {
            headers: {
                'Authorization': `Bearer ${access_token}`
            }
        });

        var profileJson = await profileResult.json();

        window.AppUser = {
            ...AppUser,
            ...profileJson.data
        };
    }

    function signUp() {
        return fetch(`${AppSettings.Server}/api/appUsers/UnionIDSignUp`, {
            headers: {
                'content-type': 'application/json'
            },
            body: JSON.stringify({
                Appid: AppSettings.appid,
                UnionID: AppUser.unionID,
                Platform: AppUser.platform.id,
                Pwd: '123456',
                Avatar: AppUser.avatar
            }),
            method: 'POST'
        });
    }

    function signIn() {
        return fetch(`${AppSettings.Server}/api/appUsers/UnionIDSignIn`, {
            headers: {
                'content-type': 'application/json'
            },
            body: JSON.stringify({
                AppID: AppSettings.appid,
                UnionID: AppUser.unionID,
                Platform: AppUser.platform.id
            }),
            method: 'POST'
        })
    }

    // 初始化APP
    window.AppOnReady = async (autoSignIn) => {

        var infoResult = await fetch(`${AppSettings.Server}/api/apps/${AppSettings.appid}/Info`);

        var _app =  await infoResult.json();

        window.App = _app.data

        document.title = App.info.name;

        $("head").prepend('<link rel="shortcut icon" type="image/x-icon" href="' + App.info.logo + '" />');

        var $iframe = $('<iframe style="display:none;" src="/favicon.ico"></iframe>');

        $iframe.on('load', () => {
            setTimeout(() => {
                $iframe.off('load').remove();
            }, 0);
        }).appendTo($('body'));

        if (App.info.tags) {
            $("head").prepend('<meta name="keywords" content="' + App.info.tags + '" />');
        }
        if (App.info.description) {
            $("head").prepend('<meta name="description" content="' + App.info.description + '" />');
        }

        if (autoSignIn == true) {
            await UnionIDSignIn();
        }

        else {
            resolve(_res.data);
        }

        var promise = new Promise((resolve, reject) => {
            resolve({ App, AppUser });
        });

        return promise;
    }


    if (AppSettings.autosignin == 'true') {
        AppOnReady(true);
    }

})(window);

;
(window => {
    var server = `${AppSettings.Server}/api/AppRank`;
    var appid = AppSettings.appid;

    // 榜单集合
    window.AppRanks = () => {
        return $.ajax({
            url: `${server}/${appid}/Report`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            }
        });
    }

    // 单个榜单数据列表
    window.AppRank = (rankKey, platform, take) => {
        var params = [
            `platform=${platform}`,
            `take=${take||30}`
        ];
        return $.ajax({
            url: `${server}/${appid}/${rankKey}?${params.join('&')}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            }
        });
    }

    // 我的排行
    window.AppRankMyIndex = (rankKey, platform, unionid) => {

        var params = [
            `platform=${platform}`,
            `unionid=${unionid}`
        ];

        return $.ajax({
            url: `${server}/${appid}/${rankKey}/Me?${params.join('&')}`,
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            }
        });
    }

    // 提交排行榜分数
    window.AppRankPostScore = (rankKey, data) => {
        return $.ajax({
            url: `${server}/${appid}/${rankKey}`,
            method: "PUT",
            headers: {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            },
            data: JSON.stringify(data)
        });
    }

})(window);

;
(window => {

    var server = `${AppSettings.Server}/api/AppStorage`;
    var appid = AppSettings.appid;

    /**
     * 查询数据
     * @param {any} _tableName 数据表名称
     * @param {any} _filter 查询条件
     * @param {any} _sort 排序
     * @param {any} _take 每页条数
     * @param {any} _skip 跳过多少条数据
     */
    window.AppStorage = function (_tableName, _filter = '', _sort = '', _take = "10", _skip = "0") {

        var queryParams = [
            `tName=${_tableName}`,
            `filter=${_filter}`,
            `sort=${_sort}`,
            `take=${_take}`,
            `skip=${_skip}`
        ]

        var settings = {
            "url": `${server}/${appid}/Query?${queryParams.join("&")}`,
            "method": "GET",
            "headers": {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            }
        };

        return $.ajax(settings);
    }

    /**
     * 添加数据
     * @param {any} _tableName 数据表名称
     * @param {any} _jsonStr json字符串
     */
    window.AppStoragePost = function (_tableName, _jsonStr = '') {

        var queryParams = [
            `tName=${_tableName}`
        ];

        var settings = {
            "url": `${server}/${appid}/Post?${queryParams.join("&")}`,
            "method": "POST",
            "headers": {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            },
            data: _jsonStr,
        };

        return $.ajax(settings);
    }

    /**
     * 删除数据
     * @param {any} _id id
     */
    window.AppStorageDelete = function (_id) {

        var queryParams = [
            `id=${_id}`
        ];

        var settings = {
            "url": `${server}/${appid}/Delete?${queryParams.join("&")}`,
            "method": "DELETE",
            "headers": {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            },
        };

        return $.ajax(settings);
    }

    /**
     * 更新数据
     * @param {any} _id id
     * @param {any} _jsonStr json字符串
     */
    window.AppStoragePut = function (_id, _jsonStr = '') {

        var queryParams = [
            `id=${_id}`
        ];

        var settings = {
            "url": `${server}/${appid}/Put?${queryParams.join("&")}`,
            "method": "PUT",
            "headers": {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            },
            "data": _jsonStr,
        };

        return $.ajax(settings);
    }

    /**
     * 数据表集合
     * */
    window.AppStorageTables = function () {
        var settings = {
            "url": `${server}/${appid}/Tables`,
            "method": "GET",
            "headers": {
                "Content-Type": "application/json",
                'Authorization': `Bearer ${access_token}`
            }
        };

        return $.ajax(settings);
    }

})(window);
