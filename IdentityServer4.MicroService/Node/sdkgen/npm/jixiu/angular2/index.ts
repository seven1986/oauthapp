// 需要配置angular4+ inersector使用
import { Injectable }  from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';
import { environment } from '@env/environment';

@Injectable()
export class IdentityServer4MicroServiceClient {
    public basePath:string = environment.ApiServer +'/identity';
    constructor(protected http: HttpClient){
    }

    public apiresource-authservers(api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/AuthServers`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public apiresource-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public apiresource-delete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public apiresource-detail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public apiresource-get(q_Name?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource`;
      let requestParams = new HttpParams();
      if (q_Name !== undefined) { requestParams = requestParams.set('q.Name', <any>q_Name);}
      if (orderby !== undefined) { requestParams = requestParams.set('orderby', <any>orderby);}
      if (asc !== undefined) { requestParams = requestParams.set('asc', <any>asc);}
      if (skip !== undefined) { requestParams = requestParams.set('skip', <any>skip);}
      if (take !== undefined) { requestParams = requestParams.set('take', <any>take);}
      if (startTime !== undefined) { requestParams = requestParams.set('startTime', <any>startTime);}
      if (endTime !== undefined) { requestParams = requestParams.set('endTime', <any>endTime);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public apiresource-post(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource`;
      return this.http.post(path, model); 
    }

    public apiresource-put(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource`;
      return this.http.put(path, model); 
    }

    public apiresource-products(api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Products`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public apiresource-publish(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource/Publish`;
      return this.http.put(path, model); 
    }

    public apiresource-publishsetting(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Publish/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public client-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/Client/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public client-delete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Client/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public client-detail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Client/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public client-get(q_ClientID?,q_ClientName?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
      const path = `${this.basePath}/Client`;
      let requestParams = new HttpParams();
      if (q_ClientID !== undefined) { requestParams = requestParams.set('q.ClientID', <any>q_ClientID);}
      if (q_ClientName !== undefined) { requestParams = requestParams.set('q.ClientName', <any>q_ClientName);}
      if (orderby !== undefined) { requestParams = requestParams.set('orderby', <any>orderby);}
      if (asc !== undefined) { requestParams = requestParams.set('asc', <any>asc);}
      if (skip !== undefined) { requestParams = requestParams.set('skip', <any>skip);}
      if (take !== undefined) { requestParams = requestParams.set('take', <any>take);}
      if (startTime !== undefined) { requestParams = requestParams.set('startTime', <any>startTime);}
      if (endTime !== undefined) { requestParams = requestParams.set('endTime', <any>endTime);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public client-post(model:any): Observable<any> {
      const path = `${this.basePath}/Client`;
      return this.http.post(path, model); 
    }

    public client-put(model:any): Observable<any> {
      const path = `${this.basePath}/Client`;
      return this.http.put(path, model); 
    }

    public codegen-clients(fromCache?,api_version?): Observable<any> {
      const path = `${this.basePath}/CodeGen/Clients`;
      let requestParams = new HttpParams();
      if (fromCache !== undefined) { requestParams = requestParams.set('fromCache', <any>fromCache);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public codegen-servers(fromCache?,api_version?): Observable<any> {
      const path = `${this.basePath}/CodeGen/Servers`;
      let requestParams = new HttpParams();
      if (fromCache !== undefined) { requestParams = requestParams.set('fromCache', <any>fromCache);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public file-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/File/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public file-image(model:any): Observable<any> {
      const path = `${this.basePath}/File/Image`;
      return this.http.post(path, model); 
    }

    public file-post(model:any): Observable<any> {
      const path = `${this.basePath}/File`;
      return this.http.post(path, model); 
    }

    public identityresource-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public identityresource-delete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public identityresource-detail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public identityresource-get(q_Name?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource`;
      let requestParams = new HttpParams();
      if (q_Name !== undefined) { requestParams = requestParams.set('q.Name', <any>q_Name);}
      if (orderby !== undefined) { requestParams = requestParams.set('orderby', <any>orderby);}
      if (asc !== undefined) { requestParams = requestParams.set('asc', <any>asc);}
      if (skip !== undefined) { requestParams = requestParams.set('skip', <any>skip);}
      if (take !== undefined) { requestParams = requestParams.set('take', <any>take);}
      if (startTime !== undefined) { requestParams = requestParams.set('startTime', <any>startTime);}
      if (endTime !== undefined) { requestParams = requestParams.set('endTime', <any>endTime);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public identityresource-post(model:any): Observable<any> {
      const path = `${this.basePath}/IdentityResource`;
      return this.http.post(path, model); 
    }

    public identityresource-put(model:any): Observable<any> {
      const path = `${this.basePath}/IdentityResource`;
      return this.http.put(path, model); 
    }

    public role-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/Role/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public role-delete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Role/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public role-detail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Role/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public role-get(api_version?): Observable<any> {
      const path = `${this.basePath}/Role`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public role-post(model:any): Observable<any> {
      const path = `${this.basePath}/Role`;
      return this.http.post(path, model); 
    }

    public role-put(model:any): Observable<any> {
      const path = `${this.basePath}/Role`;
      return this.http.put(path, model); 
    }

    public tenant-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public tenant-delete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public tenant-detail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public tenant-get(q_Host?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant`;
      let requestParams = new HttpParams();
      if (q_Host !== undefined) { requestParams = requestParams.set('q.Host', <any>q_Host);}
      if (orderby !== undefined) { requestParams = requestParams.set('orderby', <any>orderby);}
      if (asc !== undefined) { requestParams = requestParams.set('asc', <any>asc);}
      if (skip !== undefined) { requestParams = requestParams.set('skip', <any>skip);}
      if (take !== undefined) { requestParams = requestParams.set('take', <any>take);}
      if (startTime !== undefined) { requestParams = requestParams.set('startTime', <any>startTime);}
      if (endTime !== undefined) { requestParams = requestParams.set('endTime', <any>endTime);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public tenant-post(model:any): Observable<any> {
      const path = `${this.basePath}/Tenant`;
      return this.http.post(path, model); 
    }

    public tenant-put(model:any): Observable<any> {
      const path = `${this.basePath}/Tenant`;
      return this.http.put(path, model); 
    }

    public tenant-info(host?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/Info`;
      let requestParams = new HttpParams();
      if (host !== undefined) { requestParams = requestParams.set('host', <any>host);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public user-codes(api_version?): Observable<any> {
      const path = `${this.basePath}/User/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public user-delete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/User/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public user-detail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/User/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public user-get(q_roles?,q_phoneNumber?,q_name?,q_email?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
      const path = `${this.basePath}/User`;
      let requestParams = new HttpParams();
      if (q_roles !== undefined) { requestParams = requestParams.set('q.roles', <any>q_roles);}
      if (q_phoneNumber !== undefined) { requestParams = requestParams.set('q.phoneNumber', <any>q_phoneNumber);}
      if (q_name !== undefined) { requestParams = requestParams.set('q.name', <any>q_name);}
      if (q_email !== undefined) { requestParams = requestParams.set('q.email', <any>q_email);}
      if (orderby !== undefined) { requestParams = requestParams.set('orderby', <any>orderby);}
      if (asc !== undefined) { requestParams = requestParams.set('asc', <any>asc);}
      if (skip !== undefined) { requestParams = requestParams.set('skip', <any>skip);}
      if (take !== undefined) { requestParams = requestParams.set('take', <any>take);}
      if (startTime !== undefined) { requestParams = requestParams.set('startTime', <any>startTime);}
      if (endTime !== undefined) { requestParams = requestParams.set('endTime', <any>endTime);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public user-put(model:any): Observable<any> {
      const path = `${this.basePath}/User`;
      return this.http.put(path, model); 
    }

    public user-head(PhoneNumber?,api_version?): Observable<any> {
      const path = `${this.basePath}/User/Head`;
      let requestParams = new HttpParams();
      if (PhoneNumber !== undefined) { requestParams = requestParams.set('PhoneNumber', <any>PhoneNumber);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public user-register(model:any): Observable<any> {
      const path = `${this.basePath}/User/Register`;
      return this.http.post(path, model); 
    }

    public user-verifyemail(model:any): Observable<any> {
      const path = `${this.basePath}/User/VerifyEmail`;
      return this.http.post(path, model); 
    }

    public user-verifyphone(model:any): Observable<any> {
      const path = `${this.basePath}/User/VerifyPhone`;
      return this.http.post(path, model); 
    }
 }
