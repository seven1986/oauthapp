﻿;(function () {
    var portalurl = '';
    var clientSDKs = [];
    var serverSDKs = [];
    window.swaggerCodeGenSubscriptionKey = '';
    window.swaggerCodeGenToken = '';
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
    function ShowPackages(packageItems) {

         if (packageItems.length < 1) { return; }

         var itemsStr = packageItems.filter(function (x) { return x.packagePlatform == 'npm' }).map(function (x) {

             return '<tr>' +
                 '<td><a href="' + x.link + '" target="_blank">' + x.rowKey + '</a></td>' +
                 '<td><a href="' + x.link + '" target="_blank"><img src="https://img.shields.io/npm/v/' + x.icon + '.svg" /></a></td>' +
                 '<td><img src="https://img.shields.io/npm/dw/' + x.icon + '.svg" /></td>' +
                 '<td>' + x.publisher + '</td>' +
                 '</tr>';

         }).join('');

         var _layer = '<div class="panel" style="margin-top:15px">' +
             '<h4>SDK开发包 <small class="label label-success">beta</small></h4>' +
             '<div role="tabpanel">' +
             '<ul class="nav nav-tabs" role="tablist">' +
             '<li role="presentation" class="active">' +
             '<a href="#packages_npm" aria-controls="packages_npm" role="tab" data-toggle="tab">NPM</a>' +
             '</li>' +
             '</ul>' +
             '<div class="tab-content tab-content-boxed">' +
             '<div role="tabpanel" class="tab-pane active" id="packages_npm">' +
             '<div style="padding:20px" id="packagesContainer">' +
             '<table class="table">' +
             '<thead>' +
             '<tr>' +
             '<th>包</th>' +
             '<th>版本号</th>' +
             '<th>下载次数</th>' +
             '<th>作者</th>' +
             '</tr>' +
             '</thead>' +
             '<tbody>' +
             itemsStr +
             '</tbody>' +
             '</table>' +
             '</div>' +
             '</div>' +
             '</div>' +
             '</div>' +
             '</div>';

         $('#codegen_azure').before(_layer);


         var postmanBtn = '';
         var postmanChrome = packageItems.filter(x => x.packagePlatform == 'postman.chrome');
         var postmanPC = packageItems.filter(x => x.packagePlatform == 'postman.pc');
         if (postmanChrome.length == 1) {
             postmanBtn += '<li><a href="' + postmanChrome[0].link + '" target="_blank">Postman for Chrome</a></li>';
         }

         if (postmanPC.length == 1) {
             postmanBtn += '<li><a href="' + postmanPC[0].link + '" target="_blank">Postman for Windows</a></li>';
         }

         if (postmanBtn != '')
         {
             var btn = '<div class="btn-group">' +
                 document.getElementById('btnOpenConsole').outerHTML +
                 '<button type="button" class="btn btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">' +
                 '<span class="caret"></span>' +
                 '<span class="sr-only">Toggle Dropdown</span>' +
                 '</button>' +
                 '<ul class="dropdown-menu">' +
                 postmanBtn +
                 '</ul>' +
                 '</div>';

             $('#btnOpenConsole').replaceWith(btn);
         }

         var githubIssues = packageItems.filter(x => x.packagePlatform == 'github.issues');

         if (githubIssues.length == 1)
         {
             var issuesBadge = $('#cus_apiname').text() + "." + location.href.split('/')[7].replace(/\?/g, '');
             var link = githubIssues[0].link;
             var onerrorsrc = 'data:image/png;base64,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';
             $('#cus_operationname').prepend('<a class="pull-right" href="https://github.com/' + link + '/labels/' + issuesBadge + '" target="_blank"><img onerror="this.src=\'' + onerrorsrc + '\'" src="https://img.shields.io/github/issues-raw/' + link + '/' + issuesBadge + '.svg" /></a>');
         }
     }
    function ShowSubscription() {
        var _layer =
            '<button id="btnShowSubscription"' +
            'type="button"' +
            'class="btn btn-default" style="float:right;margin-left: 5px" onclick="sub_Modal()">' +
            '<span>订阅</span>' +
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
    window.codegen_clientGen = function (ele) {
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
            url: portalurl + "/ApiResource/" + apiId + "/Subscriptions/VerifyEmail",
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

     $(function ()
     {
         if ($('#apiMenu').length > 0)
         {
             var codeGenClients = portalurl + "/CodeGen/Clients",
                 codeGenServers = portalurl + "/CodeGen/Servers",
                 apiResourcePackages = portalurl + "/ApiResource/" + location.href.split('/')[5] + "/Packages";

             $('#codegen_azure').html(_template_codegen_azure);

             var codegenClientsData = localStorage.getItem('codegenClientsData');
             if (codegenClientsData == null) {
                 $.ajax({
                     type: "GET",
                     url: codeGenClients,
                     datatype: "json",
                     headers: {
                         "Ocp-Apim-Subscription-Key": swaggerCodeGenSubscriptionKey,
                         "Authorization": "Bearer " + swaggerCodeGenToken
                     },
                     contentType: "application/json; charset=utf-8"
                 }).then(r => {
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
                 $.ajax({
                     type: "GET",
                     url: codeGenServers,
                     datatype: "json",
                     headers: {
                         "Ocp-Apim-Subscription-Key": swaggerCodeGenSubscriptionKey,
                         "Authorization": "Bearer " + swaggerCodeGenToken
                     },
                     contentType: "application/json; charset=utf-8"
                 }).then(r => {
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

             $.ajax({
                 type: "GET",
                 url: apiResourcePackages,
                 datatype: "json",
                 headers: {
                     "Ocp-Apim-Subscription-Key": swaggerCodeGenSubscriptionKey,
                     "Authorization": "Bearer " + swaggerCodeGenToken
                 },
                 contentType: "application/json; charset=utf-8"
             }).done(r => {

                 if (r.code == 200) {
                     ShowPackages(r.data);
                 }

             }).fail(r => {
                 alert(JSON.stringify(r));
             });
         }
    });
})();