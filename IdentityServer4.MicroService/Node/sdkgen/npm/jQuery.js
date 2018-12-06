var template = require('./mustache.min');

module.exports = function (callback, swaggerDocumentStr, packageOptions) {
    //var result = template.render('hi, {{title}} spends {{calc}}.',
    //    {
    //        title: 'aui',
    //        calc: () => 2 + 4
    //    });

    let documentJson = JSON.parse(swaggerDocumentStr);

    let result = IGenerator(documentJson, packageOptions);

    callback(/* error */ null, result);
}

var IGenerator = function (doc, opts) {

    var sdkName = doc.info.title.replace(/\./g, "") + 'Client';

    var fns = [];

    var paths = Object.keys(doc.paths);

    paths.forEach(path => {

        let requestUrl = path;

        let pathParams = [];

        if (path.indexOf('{') > 0) {

            requestUrl = path.replace(/\{/g, '${');

            pathParams = requestUrl.split('$').filter(x => x.indexOf('{') > -1);
        }

        var methods = Object.keys(doc.paths[path]);

        methods.forEach(method => {

            let operation = doc.paths[path][method];

            let methodName = operation.operationId;
            if (methodName.indexOf('/') > -1) {
                methodName = methodName.split('/').map(x => x.substring(0, 1).toUpperCase() + x.substring(1)).join('');
            }
            else if (methodName.indexOf('-') > -1) {
                methodName = methodName.split('-').map(x => x.substring(0, 1).toUpperCase() + x.substring(1)).join('');
            }

            let fn = '';
            fn += `   /**\r\n`;
            fn += `    * @name ${operation.summary}\r\n`;
            if (operation.description) {
                fn += `    * @description ${operation.description.replace(/\r\n/g, '')}\r\n`;
            }

            let methodParams = [];
            let httpParams = [];
            let bodyParams = '';
            let apiVersionParams = '';
            if (operation.parameters != null && operation.parameters.length > 0) {
                operation.parameters.forEach(x => {
                    if (x.name == 'api-version') { apiVersionParams = 'api_version'; httpParams.push(x.name); return; }

                    if (x.in == 'path' || x.in == 'query') {
                        fn += `    * @param ${x.name}    ${x.description ? x.description : ''}\r\n`;

                        methodParams.push(x.name);

                        if (x.in == 'query') {
                            httpParams.push(x.name);
                        }
                    }
                });

                if (operation.parameters.filter(x => x.in == 'body').length > 0) {
                    bodyParams = 'model';
                }
            }

            /*
             * 特殊处理，当前网关无法导入file的operation.parameters
             * 所以维护一个静态集合，如果是存在就自动添加model参数
             */
            if (['fileimage', 'filepost', 'generaluploadimage','wechatpayuploadcert'].indexOf(methodName.toLocaleLowerCase()) > -1) {
                bodyParams = 'formData';
            }

            fn += `    */\r\n`;

            let methodParamsStr = [];
            if (methodParams.length > 0) {
                methodParamsStr.push(methodParams.map(x => x.replace("-", "_").replace(".", "_")).join(","));
            }
            if (bodyParams != '') { methodParamsStr.push(bodyParams); }
            if (apiVersionParams != '') {
                methodParamsStr.push(apiVersionParams);
            }

            fn += `    sdk.${methodName} = function(${methodParamsStr.join(",")}) {\r\n`;
            var requestUrl2 = requestUrl.replace(/\$\{/g, '\'+').replace(/\}/g, '+\'');
            requestUrl2 = '      var url = this.basepath() +\'' + requestUrl2 + '\';';
            requestUrl2 = requestUrl2.replace("+''", '');
            fn += requestUrl2 + '\r\n';

            if (httpParams.length > 0) {
                fn += `      var params = {};\r\n`;

                httpParams.forEach(p => {
                    let pName = p.replace("-", "_").replace(".", "_");

                    fn += `      if (${pName} !== undefined) { params['${p}'] =${pName} }\r\n`;
                })
                fn += `if(Object.keys(params).length>0){\r\n`;
                fn += `      url += '?'+ $.param(params)\n`;
                fn += `}\r\n`;
            }

            if (bodyParams != '') {
                if (bodyParams != 'formData') {
                    fn += `      return openapis._request({url:url,data:JSON.stringify(model),method:'${method.toUpperCase()}'});`;
                }
                else {
                    fn += `      return openapis._request({url:url,processData: false,contentType: false,data:formData,method:'${method.toUpperCase()}'});`;
                }
            }
            else {
                fn += `      return openapis._request({url:url,method:'${method.toUpperCase()}'});`;
            }


            fn += ` \r\n    }`;

            fns.push(fn);
        });
    });

    var fnStr = `;(function () {
      var sdk = new Object();
          sdk.basepath = function()
            { 
              return openapis._settings.server_endpoint + '${doc.basePath}'; 
            };\r\n\r\n` +
        fns.join('\n\n') +
        `\r\n
            window.openapis.${sdkName} = sdk;
            })()`;

    return fnStr;
}