;(function () { 

var clientSDKs = [];

var serverSDKs = [];

var swaggerUrl = location.href.substr(0, location.href.indexOf('/operations/')) + '/export?DocumentFormat=Swagger';

var swaggerCodeGenCurrentItem = {};

var swaggerCodeGenCurrentItemIsServer = {};

function ShowClientSDKs()
{
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
        _layer += '<li><a onclick="showCodeGenModal(' + ind + ')">' + r.language + '</a></li>';
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
        _layer += '<li><a onclick="showCodeGenModal(' + ind + ',true)">' + r.language + '</a></li>';
    });

    _layer += '</ul>' +
        '</div>';

    $('#apiMenu').parent().after(_layer);
}

function gen(path, options) {
    return $.ajax({
        type: "POST",
        url: "https://generator.swagger.io/api/gen/" + path,
        data: JSON.stringify(options),
        datatype: "json",
        contentType: "text/json;charset=UTF-8"
    });
}

function showCodeGenModal(ind, isServer) {
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

function clientGen(ele) {
    var data = {
        options: {},
        swaggerUrl: swaggerUrl,
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

    gen("clients/" + swaggerCodeGenCurrentItem.language, data).then(r => {
        $("#CodePackageResult").html('<h4>code</h4><input type="text" class="form-control" value="' + r.code + '" /><h4>link</h4><input type="text" class="form-control" value="' + r.link + '" />');

        $("#CodePackageModal").modal('show');
    });
}

var hosturl = location.protocol + location.host;

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
                                  <button type="button" class="btn btn-success" onclick="clientGen()">下载SDK</button>
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
            localStorage.setItem('codegenClientsData',JSON.stringify(clientSDKs));
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
});
})();