var template = require('./mustache.min');

module.exports = function (callback, doc, second)
{
    //var result = template.render('hi, {{title}} spends {{calc}}.',
    //    {
    //        title: 'aui',
    //        calc: () => 2 + 4
    //    });

    var result = angular2Gen(JSON.parse(doc));

    callback(/* error */ null, result);
}

var angular2Gen = function (doc) {

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

            let k = `${path}/${method}`;

            let operation = doc.paths[path][method];

            let methodName = operation.operationId.replace(/\//g, '');

            let methodParams = [];

            let httpParams = [];

            if (operation.parameters != null && operation.parameters.length > 0) {

                methodParams = operation.parameters.map(x => x.name);

                methodParams.filter(x => {
                    let isPathParamater = pathParams.filter(pp => pp.indexOf('{' + x + '}') > -1).length > 0;

                    if (!isPathParamater) {
                        httpParams.push(x);
                    }
                })
            }
            // GET/DELETE 请求
            let fn = '';
            if (method == 'get' || method == 'delete') {
                fn = `    public ${methodName}(${methodParams.length > 0 ? methodParams.map(x => x.replace("-", "_").replace(".", "_")).join("?,") + '?' : ''}): Observable<any> {\r\n`;

                fn += '      const path = `${this.basePath}' + requestUrl + '`;\r\n';

                if (httpParams.length > 0) {
                    fn += `      let requestParams = new HttpParams();\r\n`;

                    httpParams.forEach(p => {
                        fn += `      if (${p.replace("-", "_").replace(".", "_")} !== undefined) { requestParams = requestParams.set('${p}', <any>${p.replace("-", "_").replace(".", "_")});}\r\n`;
                    })

                    fn += `      let options = { params: requestParams }\n`;

                    fn += `      return this.http.${method}(path, options);`;
                }

                else {
                    fn += `      return this.http.${method}(path);`;
                }

                fn += ` \r\n    }`;
            }

            // PUT/POST 请求
            else if (method == 'post' || method == 'put') {
                fn += `    public ${methodName}(model:any): Observable<any> {\r\n`;

                fn += '      const path = `${this.basePath}' + requestUrl + '`;\r\n';

                fn += `      return this.http.${method}(path, model);`;

                fn += ` \r\n    }`;
            }


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