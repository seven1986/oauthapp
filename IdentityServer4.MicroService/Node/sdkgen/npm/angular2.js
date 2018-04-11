var template = require('./mustache.min');

module.exports = function (callback, swaggerDocumentStr, packageOptions) {
    //var result = template.render('hi, {{title}} spends {{calc}}.',
    //    {
    //        title: 'aui',
    //        calc: () => 2 + 4
    //    });

    try {
        let documentJson = JSON.parse(swaggerDocumentStr);


        let result = IGenerator(documentJson, packageOptions);

        callback(/* error */ null, result);
    }
    catch (err) {
        callback(err, null);
    }
}

var IGenerator = function (doc,opts) {

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
                    fn += `      /**\r\n`;
                    fn += `       * @name ${operation.description}\r\n`;

                    let methodParams = [];
                    let httpParams = [];
                    let bodyParams = '';
                    let apiVersionParams = '';
                    if (operation.parameters != null && operation.parameters.length > 0)
                    {
                        operation.parameters.forEach(x =>
                        {
                            if (x.name == 'api-version') { apiVersionParams = 'api_version?'; httpParams.push(x.name); return; }

                            if (x.in == 'path' || x.in == 'query') {
                                fn += `       * @param ${x.name}    ${x.description ? x.description : ''}\r\n`;

                                methodParams.push(x.name);

                                if (x.in == 'query') {
                                    httpParams.push(x.name);
                                }
                            }
                        });

                        if (operation.parameters.filter(x => x.in == 'body').length > 0) {
                            bodyParams = 'model?:any';
                        }
                    }

                    /*
                     * 特殊处理，当前网关无法导入file的operation.parameters
                     * 所以维护一个静态集合，如果是存在就自动添加model参数
                     */
                    if (['fileimage', 'filepost'].indexOf(methodName.toLocaleLowerCase()) > -1) {
                        bodyParams = 'model?:any';
                    }

                    fn += `       */\r\n`;

                    let methodParamsStr = [];
                    if (methodParams.length > 0){
                        methodParamsStr.push(methodParams.map(x => x.replace("-", "_").replace(".", "_")).join("?,")+'?');
                    }
                    if (bodyParams != '') { methodParamsStr.push(bodyParams);}
                    if (apiVersionParams != '') {
                        methodParamsStr.push(apiVersionParams);
                    }

                    fn += `    public ${methodName}(${methodParamsStr.join(",")}): Observable<any> {\r\n`;

                    fn += '      const path = `${this.basePath}' + requestUrl + '`;\r\n';

                    if (httpParams.length > 0)
                    {
                        fn += `      let requestParams = new HttpParams();\r\n`;

                        httpParams.forEach(p => {
                            let pName = p.replace("-", "_").replace(".", "_");

                            fn += `      if (${pName} !== undefined) { requestParams = requestParams.set('${p}', <any>${pName});}\r\n`;
                        })

                        fn += `      let options = { params: requestParams }\n`;
                    }

                    if (bodyParams != '') {
                        if (httpParams.length > 0) {
                            fn += `      return this.http.${method}(path, model, options);`;
                        }
                        else {
                            fn += `      return this.http.${method}(path, model);`;
                        }
                    }
                    else if (httpParams.length > 0) {
                        fn += `      return this.http.${method}(path, options);`;
                    }
                    else {
                        fn += `      return this.http.${method}(path);`;
                    }

                    fn += ` \r\n    }`;

                    fns.push(fn);
                });
            });

            var fnStr = `// 需要配置angular4+ inersector使用
import { Injectable }  from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { environment } from '@env/environment';

@Injectable()
export class ${sdkName} {
    public basePath:string = environment.ApiServer +'${doc.basePath}';
    constructor(protected http: HttpClient){
    }\r\n\r\n`+
                fns.join('\n\n') +
                `\r\n }`;

            return fnStr;
        }