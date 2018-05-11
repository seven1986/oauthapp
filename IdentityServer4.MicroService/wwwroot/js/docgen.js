var swaggerRoot = 'https://portal.ixingban.com/docs/services/1/';

var IGenerator = function (doc, opts) {
    var fns = [];
    var operations = [];
    var groupsLevel1 = [];
    var groupsLevel2 = [];

    Object.keys(doc.paths).forEach(path => {
        var methods = Object.keys(doc.paths[path]);

        methods.forEach(method => {
            var operation = doc.paths[path][method];
            var operationId = operation.operationId;
            var summary = operation.summary;
            var description = operation.description;
            var groups = summary.split(' - ');
            var id = operationId.split('-')[0];

            if (groups.length > 1) {
                //加入父级菜单
                if (groupsLevel1.indexOf(groups[0]) <= -1) {
                    operations.push({
                        id: id,
                        name: groups[0],
                        items: []
                    });
                    groupsLevel1.push(groups[0]);
                }

                for (var i = 0; i < operations.length; i++) {
                    var xid = operations[i].id;

                    if (xid.split('-').indexOf(id) > -1 && summary.indexOf(operations[i].name) > -1) {

                        var name = summary.replace(operations[i].name + ' - ', '');
                        var desc = description;
                        var _groups = name.split(' - ');

                        operations[i].items.push({
                            id: operationId,
                            name: name,
                            desc: desc
                        });

                        break;
                    }
                }
            }
        });
    });



    operations.forEach(operation => {
        var operations2 = [];

        operation.items.forEach((x, index) => {
            let groups = x.name.split(' - ');

            if (groups.length > 1) {
                //加入父级菜单
                if (groupsLevel2.indexOf(groups[0]) <= -1) {
                    operations2.push({
                        name: groups[0],
                        items: []
                    });
                    groupsLevel2.push(groups[0]);
                }

                for (var i = 0; i < operations2.length; i++) {
                    if (x.name.indexOf(operations2[i].name) > -1) {
                        let name = x.name.replace(groups[0] + ' - ', '');
                        name = name.replace(groups[1] + ' - ', '');

                        operations2[i].items.push({
                            id: x.id,
                            name: name,
                            desc: x.desc
                        });
                        break;
                    }
                }

                delete operation.items[index];
            }
        });

        operations2.forEach(x => {
            operation.items.push(x);
        });
        operation.items = operation.items.filter(x => x != null);
    });

    //return JSON.stringify(operations, '', '   ');

    operations.forEach(x => {
        var fn = x.name + '\r\n----------------------\r\n\r\n';

        x.items.forEach(xx => {
            if (xx.items && xx.items.length > 0) {
                fn += xx.name + '\r\n~~~~~~~~~~~~~~~~~~~~~~\r\n\r\n';

                xx.items.forEach(xxx => {
                    fn += xxx.name + '\r\n^^^^^^^^^^^^^^^^^^^^^^^^^^^\r\n\r\n';
                    fn += '.. raw:: html\r\n\n\t';
                    fn += '<p>\r\n\t';
                    fn += xxx.desc.replace(/\n/g, '\t') +
                        '\r\n\t<br /><br />' +
                        '\r\n\t<a class="btn btn-neutral" href="' + swaggerRoot + 'operations/' + xxx.id + '">Link</a>' +
                        '\r\n\t</p>\r\n' +
                        '\r\n|\r\n\r\n';
                });
            }

            else {
                fn += xx.name + '\r\n~~~~~~~~~~~~~~~~~~~~~~\r\n\r\n';
                fn += '.. raw:: html\r\n\n\t';
                fn += '<p>\r\n\t';
                fn += xx.desc.replace(/\n/g, '\t') +
                    '\r\n\t<br /><br />' +
                    '\r\n\t<a class="btn btn-neutral" href="' + swaggerRoot + 'operations/' + xx.id + '">Link</a>' +
                    '\r\n\t</p>\r\n' +
                    '\r\n|\r\n\r\n';
            }

        })

        fns.push(fn);
    });

    var fnStr = doc.info.title +
        '\r\n=================\r\n' +
        '.. Note::\r\n\r\n\t' +
        doc.info.description + '\r\n\r\n' +
        fns.join('\r\n');

    return fnStr;
}


$(function () {

    var swaggerUrl = swaggerRoot + 'export?DocumentFormat=Swagger';

    $.getJSON(swaggerUrl).then(doc => {
        window.doc = doc;

        var resultSDK = IGenerator(doc);

        $('#result').text(resultSDK);

        hljs.initHighlighting();
    });
});