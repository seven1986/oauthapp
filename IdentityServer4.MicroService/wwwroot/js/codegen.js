; (function () {
    var clientSDKs = [];
    var serverSDKs = [];

    window.swaggerCodeGenToken = 'eyJhbGciOiJSUzI1NiIsImtpZCI6IjQxQjg0RkEwRTM4MEVGMzVEMTkxREFGNjczQjFENkMwRTgzQUY0RkYiLCJ0eXAiOiJKV1QiLCJ4NXQiOiJRYmhQb09PQTd6WFJrZHIyYzdIV3dPZzY5UDgifQ.eyJuYmYiOlsxNTIzOTYxNzk2LDE1MjM5NjE3OTFdLCJleHAiOlsxNTI3NTYxNzk2LDE1MjM5NjUzOTFdLCJpc3MiOlsiaHR0cHM6Ly9pZHMuaml4aXVjbG91ZC5jbiIsImh0dHBzOi8vaWRzLmppeGl1Y2xvdWQuY24iXSwiYXVkIjpbImh0dHBzOi8vaWRzLmppeGl1Y2xvdWQuY24vcmVzb3VyY2VzIiwiaWRzNC5tcyJdLCJjbGllbnRfaWQiOiJ0ZXN0Iiwic3ViIjoiMiIsImF1dGhfdGltZSI6MTUyMzk2MTY4NywiaWRwIjoibG9jYWwiLCJwZXJtaXNzaW9uIjoiaWRzNC5tcy5jbGllbnQuaXNzdWV0b2tlbiIsInJvbGUiOlsidXNlcnMiLCJkZXZlbG9wZXIiXSwiY2xpZW50X3RlbmFudCI6eyJpZCI6MSwibmFtZSI6IuWQieengCIsImNsYWltcyI6e319LCJzY29wZSI6WyJvcGVuaWQiLCJwcm9maWxlIiwiaWRzNC5tcy5hbGwiXSwiYW1yIjpbInB3ZCJdfQ.LqqW3Gf2-OW8YpzuCcstaVsVtMBQ-lX2eMsJds6L_S2fiCXK8p1qTbH0L6l8u3LOdP4LH7ryBPkfNxqDT04QIofdsz2YPcD0P2ktpfqDEPv2mUyoUhjhs8ItgzuOS4p9lewXv_0nLglGeVDdBMhtAU1AI1-19Y-aLIxq-sA6MkmRdbjQbWHMTQvdje5OlP5nfzOzgwdZARduyP8ndnhoa3R1Bs6tlbEhfPPi-ox3OZbXMveavYz99FqI_NU_FQmwa0nzSLV4fO2hDfKPXIBDJRaxNECXl3ZlPPobABmAxTnr0vUuRbe_1GIdPSDW4OUVyWhmH4kyAlwJmAzwEMkC_w';
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
            'class="btn btn-default" style="float:right;margin-left: 5px">' +
            '<span class="glyphicon glyphicon-star-empty"></span> 订阅' +
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