; (function () {
    var clientSDKs = [];
    var serverSDKs = [];
    window.swaggerCodeGenSubscriptionKey = '35a0672e5ff94b72a2e658e3debb2237';
    /**
     * token expiredAt 1555560241
     */
    window.swaggerCodeGenToken = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjQxQjg0RkEwRTM4MEVGMzVEMTkxREFGNjczQjFENkMwRTgzQUY0RkYiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJRYmhQb09PQTd6WFJrZHIyYzdIV3dPZzY5UDgifQ.eyJuYmYiOjE1MjQwMjQyNDEsImV4cCI6MTU1NTU2MDI0MSwiaXNzIjoiaHR0cHM6Ly9pZHMuaml4aXVjbG91ZC5jbiIsImF1ZCI6WyJodHRwczovL2lkcy5qaXhpdWNsb3VkLmNuL3Jlc291cmNlcyIsImlkczQubXMiXSwiY2xpZW50X2lkIjoidGVzdCIsInN1YiI6IjIiLCJhdXRoX3RpbWUiOjE1MjQwMjQxNDcsImlkcCI6ImxvY2FsIiwicGVybWlzc2lvbiI6ImlkczQubXMuY2xpZW50Lmlzc3VldG9rZW4saWRzNC5tcy5hcGlyZXNvdXJjZS52ZXJpZnllbWFpbCIsInJvbGUiOlsidXNlcnMiLCJkZXZlbG9wZXIiXSwiY2xpZW50X3RlbmFudCI6eyJpZCI6MSwibmFtZSI6IuWQieengCIsImNsYWltcyI6e319LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiaWRzNC5tcy5hbGwiXSwiYW1yIjpbInB3ZCJdfQ.NH0I1Cgl0bVecmbzM9LWR_7xp5anxNQJNm_eaEBWITw13IOddzxhf4YO2zBsLcXkL6c_gmgtosClo22_6zuCsQw-XZFUOQe6KhOsg71Hd_jpsF-S_JYswdzbsP0hcp3Yf66kuK_n6HHykwOby7A8WkjTShlvIbbTGSl37R11MGOIGogtQWP5F3zvild55CtwyzI5aN6zG8Dj0WKV4aUEj-aUFM3874LHcujXp-TNStnJGnjD2FBqCnxMVCxUnLe9qUPcVbenLM7Pz3SdzceIV2ZoGhSooiVtFetaL_qUZQHLN2O_vVbeKKReA9er993GB6l8OMW3WJkLff7vy7bbXg';
    window.swaggerCodeGenCurrentItem = {};
    window.swaggerCodeGenCurrentItemIsServer = {};

    function ShowClientSDKs() {
        var _layer = '<div class="btn-group pull-right" role="group" style="margin-right:5px">' +
            '<button id="btnGroupClientsSDKDrop"' +
            'type="button"' +
            'class="btn btn-default dropdown-toggle"' +
            'data-toggle="dropdown"' +
            'aria-haspopup="true"' +
            'aria-expanded="false">' +
            '<span class="glyphicon glyphicon-download-alt"></span> Clients' +
            '</button>' +
            '<ul class="dropdown-menu" aria-labelledby="btnGroupClientsSDKDrop">';

        clientSDKs.forEach((r, ind) => {
            _layer += '<li><a onclick="codegen_Modal(' + ind + ')">' + r.language + '</a></li>';
        });

        _layer += '</ul>' +
            '</div>';

        $('#apiMenu').parent().after(_layer);
    }
    function ShowServerSDKs() {

        var _layer = '<div class="btn-group pull-right" role="group" style="margin-right:5px">' +
            '<button id="btnGroupClientsSDKDrop"' +
            'type="button"' +
            'class="btn btn-default dropdown-toggle"' +
            'data-toggle="dropdown"' +
            'aria-haspopup="true"' +
            'aria-expanded="false">' +
            '<span class="glyphicon glyphicon-download-alt"></span> Servers' +
            '</button>' +
            '<ul class="dropdown-menu" aria-labelledby="btnGroupClientsSDKDrop">';

        serverSDKs.forEach((r, ind) => {
            _layer += '<li><a onclick="codegen_Modal(' + ind + ',true)">' + r.language + '</a></li>';
        });

        _layer += '</ul>' +
            '</div>';

        $('#apiMenu').parent().after(_layer);
    }

    function ShowSubscription() {
        var _layer =
            '<button id="btnShowSubscription"' +
            'type="button"' +
            'class="btn btn-default" style="float:right;margin-left: 5px" onclick="sub_Modal()">' +
            '<span class="glyphicon glyphicon-star"></span> 订阅' +
            '</button>';

        $('#apiMenu').parent().before(_layer);
    }

    window.codegen_Modal = function (ind, isServer) {
        let i;

        if (isServer) {
            swaggerCodeGenCurrentItemIsServer = true;
            i = swaggerCodeGenCurrentItem = serverSDKs[ind];
        }
        else {
            swaggerCodeGenCurrentItemIsServer = false;
            i = swaggerCodeGenCurrentItem = clientSDKs[ind];
        }

        var eles = [];

        for (var v in i.options) {
            if (i.options[v].type == 'boolean') {
                eles.push('<div class="form-group">' +
                    '<div class="checkbox">' +
                    '<label><input name="' + i.options[v].opt + '" type="checkbox"' + (i.options[v].default == 'true' ? ' checked="checked"' : '') + '> ' + i.options[v].opt + '</label>' +
                    '</div>' +
                    '<p class="help-block">' + i.options[v].description + '</p></div>');
            }

            else {
                eles.push('<div class="form-group"><label>' + i.options[v].opt + '</label>' +
                    '<input type="text" name="' + i.options[v].opt + '" class="form-control" value="' + (i.options[v].default ? i.options[v].default : '') + '">' +
                    '<p class="help-block">' + i.options[v].description + '</p>' +
                    '</div>');
            }
        }

        $('#CodegenModal .modal-title').html('<b>' + i.language + '</b> - ' + (isServer ? "Servers" : "Clients"));
        $("#languageoptions").html(eles.join(''));
        $('#CodegenModal').modal('show');
    }
    window.codegen_clientGen = function (ele)
    {
        var data = {
            options: {},
            swaggerUrl: location.href.substr(0, location.href.indexOf('/operations/')) + '/export?DocumentFormat=Swagger',
        };

        $("#languageoptions input").each((index, ipt) => {
            if (ipt.attributes.type.value == 'checkbox') {
                data.options[ipt.attributes.name.value] = ipt.checked;
            }

            else {
                data.options[ipt.attributes.name.value] = ipt.value;
            }
        });

        $("#CodegenModal").modal('hide');

        $.ajax({
            type: "POST",
            url: "https://generator.swagger.io/api/gen/clients/" + swaggerCodeGenCurrentItem.language,
            data: JSON.stringify(data),
            datatype: "json",
            contentType: "text/json;charset=UTF-8"
        }).done(r => {
            var htmlcode = '<h4>code</h4><input type="text" class="form-control" value="' + r.code + '" /><h4>link</h4><input type="text" class="form-control" value="' + r.link + '" />';
            $("#CodePackageResult").html(htmlcode);
            $("#CodePackageModal").modal('show');
        }).fail(r => {
            alert(JSON.stringify(r));
        });
    }

    window.sub_Modal = function ()
    {
        $('#subModal').modal('show');
    }

    window.postSubscriber = function () {
        var emailTest = /^([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+@([a-zA-Z0-9]+[_|\_|\.]?)*[a-zA-Z0-9]+\.[a-zA-Z]{2,3}$/;
        var email = $('#subscriberEmail').val();
        if (!emailTest.test(email)) {
            alert('邮箱格式错误');
            return;
        }
        $('#subModal').modal('hide');
        var apiId = location.href.replace(location.protocol + '//' + location.host + '/docs/services/', '').split('/')[0];
        
        $.ajax({
            type: "POST",
            url: "https://openapis.ixingban.com/identity/ApiResource/" + apiId + "/Subscriptions/VerifyEmail",
            data: JSON.stringify({ email: email }),
            datatype: "json",
            headers: {
                "Ocp-Apim-Subscription-Key": swaggerCodeGenSubscriptionKey,
                "Authorization": "Bearer " + swaggerCodeGenToken
            },
            contentType: "application/json; charset=utf-8"
        }).done(r => {
            alert('请前往邮箱激活订阅即可。');
        }).fail(r => {
            alert(JSON.stringify(r));
        });
    }

    //var hosturl = location.protocol + location.host;

    const _template_codegen_azure = `<div>
                  <div id="CodegenModal" class="modal fade">
                      <div class="modal-dialog">
                          <div class="modal-content">
                              <div class="modal-header">
                                  <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                  <h4 class="modal-title">Modal title</h4>
                              </div>
                              <div class="modal-body">
                                  <form id="languageoptions"></form>
                              </div>
                              <div class="modal-footer">
                                  <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
                                  <button type="button" class="btn btn-success" onclick="codegen_clientGen()">下载SDK</button>
                              </div>
                          </div>
                      </div>
                  </div>

                  <div id="CodePackageModal" class="modal fade">
                      <div class="modal-dialog">
                          <div class="modal-content">
                              <div class="modal-header">
                                  <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                  <h4 class="modal-title">生成SDK成功！</h4>
                              </div>
                              <div class="modal-body">
                                  <div class="alert alert-success">请复制下面的地址，下载SDK包</div>
                                  <div id="CodePackageResult"></div>
                              </div>
                              <div class="modal-footer">
                                  <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                              </div>
                          </div>
                      </div>
                  </div>

                    <div id="subModal" class="modal fade">
                      <div class="modal-dialog">
                          <div class="modal-content">
                              <div class="modal-header">
                                  <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                                  <h4 class="modal-title">订阅服务动态</h4>
                              </div>
                              <div class="modal-body">
                                  <div class="form-group">
                                        <label>邮箱地址</label>
                                        <input type="text" name="subscriberEmail" class="form-control" id="subscriberEmail">
                                        <p class="help-block">提交后，将会发送1封验证邮件到您的邮箱。</p>
                                  </div>
                              </div>
                              <div class="modal-footer">
                                  <button type="button" class="btn btn-default" data-dismiss="modal">关闭</button>
                                  <button type="button" class="btn btn-success" onclick="postSubscriber()">确认</button>
                              </div>
                          </div>
                      </div>
                  </div>
              </div>`;

    $(function () {
        $('#codegen_azure').html(_template_codegen_azure);

        var codegenClientsData = localStorage.getItem('codegenClientsData');
        if (codegenClientsData == null) {
            $.getJSON("https://ids.jixiucloud.cn/CodeGen/Clients").then(r => {
                clientSDKs = r.data;
                ShowClientSDKs();
                localStorage.setItem('codegenClientsData', JSON.stringify(clientSDKs));
            });
        }
        else {
            clientSDKs = JSON.parse(codegenClientsData);
            ShowClientSDKs();
        }

        var codegenServersData = localStorage.getItem('codegenServersData');
        if (codegenServersData == null) {
            $.getJSON("https://ids.jixiucloud.cn/CodeGen/Servers").then(r => {
                serverSDKs = r.data;
                ShowServerSDKs();
                localStorage.setItem('codegenServersData', JSON.stringify(serverSDKs));
            });
        }
        else {
            serverSDKs = JSON.parse(codegenServersData);
            ShowServerSDKs();
        }

        ShowSubscription();
    });
})();