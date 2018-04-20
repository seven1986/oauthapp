﻿; (function () {

    var clientSDKs = [];
    var serverSDKs = [];
    window.swaggerCodeGenSubscriptionKey = '28f9cac6b28348a0b3950603eee5b2ec';
    window.swaggerCodeGenToken = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjQxQjg0RkEwRTM4MEVGMzVEMTkxREFGNjczQjFENkMwRTgzQUY0RkYiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJRYmhQb09PQTd6WFJrZHIyYzdIV3dPZzY5UDgifQ.eyJuYmYiOjE1MjQyMDY0NjYsImV4cCI6MTU1NTc0MjQ2NiwiaXNzIjoiaHR0cHM6Ly9pZHMuc2hpbmdzb3UuY29tIiwiYXVkIjpbImh0dHBzOi8vaWRzLnNoaW5nc291LmNvbS9yZXNvdXJjZXMiLCJpZHM0Lm1zIl0sImNsaWVudF9pZCI6InRlc3QiLCJzdWIiOiIyIiwiYXV0aF90aW1lIjoxNTI0MjA2NDYxLCJpZHAiOiJsb2NhbCIsInBlcm1pc3Npb24iOiJpZHM0Lm1zLmFwaXJlc291cmNlLnZlcmlmeWVtYWlsLGlkczQubXMuYXBpcmVzb3VyY2UucGFja2FnZXMsaWRzNC5tcy5hcGlyZXNvdXJjZS5wb3N0cGFja2FnZXMsaWRzNC5tcy5hcGlyZXNvdXJjZS5kZWxldGVwYWNrYWdlLGlkczQubXMuY2xpZW50Lmlzc3VldG9rZW4iLCJyb2xlIjpbInVzZXJzIiwiZGV2ZWxvcGVyIl0sImNsaWVudF90ZW5hbnQiOnsiaWQiOjEsIm5hbWUiOiLml63lrofnianogZTnvZEiLCJjbGFpbXMiOnt9fSwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsImlkczQubXMuYWxsIl0sImFtciI6WyJwd2QiXX0.PWb5Wp79rc_N50Q0erbvqIpdlbzGo0958bfH_lNqO78_cOFtgpLm-pl-gGwMWKNEl5ZdgeZpJz2lfhY8syIaRh-hZmS3RY_9z4DcpbIc2Cmeyv2Y2xOjQ0DRhIgfgc89p0cIWtIl0oF7gpV76raP4ka0yI1WXKUdddprqTinfDegiZRXwLkSsLiir8IzwvtHqJP_GAufJWOGAy886JDL2QcX1zp-JrtDibawPyOpOpJhlV21TjWJ0EcYhooKuuNP9SmBBXRtODO5-qcbK0oVaf_UhSZ1h6Fqjbjf0C1Z0a_UDzMuiXpy7Ya3EsuFcaHIVxQTDjvUbuuJnGqTvjPXIg';
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

        var itemsStr= packageItems.map(x => {
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
            '<table class="table">'+
                '<thead>'+
                    '<tr>'+
                        '<th>包</th>'+
                        '<th>版本号</th>'+
                        '<th>下载次数</th>'+
                        '<th>作者</th>'+
                    '</tr>'+
                '</thead>'+
                '<tbody>'+
            itemsStr+
                '</tbody>'+
            '</table>'+
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>' +
            '</div>';

         $('#codegen_azure').before(_layer);
    }

    function ShowSubscription() {
        var _layer =
            '<button id="btnShowSubscription"' +
            'type="button"' +
            'class="btn btn-default" style="float:right;margin-left: 5px" onclick="sub_Modal()">' +
            '<span class="glyphicon glyphicon-star"></span>' +
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
         var doc = Customization.dataModel['DocumentationOperationDetails'].operation;
         var requestHost = doc.scheme + '//' + doc.host;

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
            url: requestHost + "/identity/ApiResource/" + apiId + "/Subscriptions/VerifyEmail",
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
             var doc = Customization.dataModel['DocumentationOperationDetails'].operation,
                 codeGenClients = "https://" + doc.host + "/identity/CodeGen/Clients",
                 codeGenServers = "https://" + doc.host + "/identity/CodeGen/Servers",
                 apiResourcePackages = "https://" + doc.host + "/identity/ApiResource/" + doc.api.id + "/Packages";

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