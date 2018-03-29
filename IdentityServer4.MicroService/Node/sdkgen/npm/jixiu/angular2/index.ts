
// 需要配置angular4+ inersector使用
import {Injectable }  from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs/Observable';

@Injectable()
export class IdentityServer4MicroServiceClient {
    public basePath:string = 'undefined';
    constructor(protected http: HttpClient){
    }

    public ApiResourceGet(q_name?,q_expandScopes?,q_expandClaims?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource`;
      let requestParams = new HttpParams();
      if (q_name !== undefined) { requestParams = requestParams.set('q.name', <any>q_name);}
      if (q_expandScopes !== undefined) { requestParams = requestParams.set('q.expandScopes', <any>q_expandScopes);}
      if (q_expandClaims !== undefined) { requestParams = requestParams.set('q.expandClaims', <any>q_expandClaims);}
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

    public ApiResourcePut(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource`;
      return this.http.put(path, model); 
    }

    public ApiResourcePost(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource`;
      return this.http.post(path, model); 
    }

    public ApiResourceDetail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ApiResourceDelete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public ApiResourcePublish(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource/Publish`;
      return this.http.put(path, model); 
    }

    public ApiResourcePublishSetting(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Publish/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ApiResourceVersions(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Versions/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ApiResourcePublishRevision(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource/Revisions`;
      return this.http.post(path, model); 
    }

    public ApiResourcePublishVersion(model:any): Observable<any> {
      const path = `${this.basePath}/ApiResource/Versions`;
      return this.http.post(path, model); 
    }

    public ApiResourceAuthServers(api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/AuthServers`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ApiResourceProducts(api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Products`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ApiResourceCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/ApiResource/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ClientGet(q_ClientID?,q_ClientName?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
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

    public ClientPut(model:any): Observable<any> {
      const path = `${this.basePath}/Client`;
      return this.http.put(path, model); 
    }

    public ClientPost(model:any): Observable<any> {
      const path = `${this.basePath}/Client`;
      return this.http.post(path, model); 
    }

    public ClientDetail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Client/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public ClientDelete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Client/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public ClientCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/Client/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public CodeGenClients(fromCache?,api_version?): Observable<any> {
      const path = `${this.basePath}/CodeGen/Clients`;
      let requestParams = new HttpParams();
      if (fromCache !== undefined) { requestParams = requestParams.set('fromCache', <any>fromCache);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public CodeGenServers(fromCache?,api_version?): Observable<any> {
      const path = `${this.basePath}/CodeGen/Servers`;
      let requestParams = new HttpParams();
      if (fromCache !== undefined) { requestParams = requestParams.set('fromCache', <any>fromCache);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public CodeGenGenerateClient(model:any): Observable<any> {
      const path = `${this.basePath}/CodeGen/GenerateClient`;
      return this.http.post(path, model); 
    }

    public FilePost(model:any): Observable<any> {
      const path = `${this.basePath}/File`;
      return this.http.post(path, model); 
    }

    public FileImage(model:any): Observable<any> {
      const path = `${this.basePath}/File/Image`;
      return this.http.post(path, model); 
    }

    public FileCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/File/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public IdentityResourceGet(q_Name?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
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

    public IdentityResourcePut(model:any): Observable<any> {
      const path = `${this.basePath}/IdentityResource`;
      return this.http.put(path, model); 
    }

    public IdentityResourcePost(model:any): Observable<any> {
      const path = `${this.basePath}/IdentityResource`;
      return this.http.post(path, model); 
    }

    public IdentityResourceDetail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public IdentityResourceDelete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public IdentityResourceCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/IdentityResource/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public RoleGet(api_version?): Observable<any> {
      const path = `${this.basePath}/Role`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public RolePut(model:any): Observable<any> {
      const path = `${this.basePath}/Role`;
      return this.http.put(path, model); 
    }

    public RolePost(model:any): Observable<any> {
      const path = `${this.basePath}/Role`;
      return this.http.post(path, model); 
    }

    public RoleDetail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Role/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public RoleDelete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Role/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public RoleCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/Role/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public TenantGet(q_Host?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
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

    public TenantPut(model:any): Observable<any> {
      const path = `${this.basePath}/Tenant`;
      return this.http.put(path, model); 
    }

    public TenantPost(model:any): Observable<any> {
      const path = `${this.basePath}/Tenant`;
      return this.http.post(path, model); 
    }

    public TenantDetail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public TenantDelete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public TenantInfo(host?,api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/Info`;
      let requestParams = new HttpParams();
      if (host !== undefined) { requestParams = requestParams.set('host', <any>host);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public TenantCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/Tenant/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public UserGet(q_roles?,q_phoneNumber?,q_name?,q_email?,orderby?,asc?,skip?,take?,startTime?,endTime?,api_version?): Observable<any> {
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

    public UserPut(model:any): Observable<any> {
      const path = `${this.basePath}/User`;
      return this.http.put(path, model); 
    }

    public UserPost(model:any): Observable<any> {
      const path = `${this.basePath}/User`;
      return this.http.post(path, model); 
    }

    public UserDetail(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/User/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public UserDelete(id?,api_version?): Observable<any> {
      const path = `${this.basePath}/User/${id}`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.delete(path, options); 
    }

    public UserHead(PhoneNumber?,api_version?): Observable<any> {
      const path = `${this.basePath}/User/Head`;
      let requestParams = new HttpParams();
      if (PhoneNumber !== undefined) { requestParams = requestParams.set('PhoneNumber', <any>PhoneNumber);}
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }

    public UserRegister(model:any): Observable<any> {
      const path = `${this.basePath}/User/Register`;
      return this.http.post(path, model); 
    }

    public UserVerifyPhone(model:any): Observable<any> {
      const path = `${this.basePath}/User/VerifyPhone`;
      return this.http.post(path, model); 
    }

    public UserVerifyEmail(model:any): Observable<any> {
      const path = `${this.basePath}/User/VerifyEmail`;
      return this.http.post(path, model); 
    }

    public UserCodes(api_version?): Observable<any> {
      const path = `${this.basePath}/User/Codes`;
      let requestParams = new HttpParams();
      if (api_version !== undefined) { requestParams = requestParams.set('api-version', <any>api_version);}
      let options = { params: requestParams }
      return this.http.get(path, options); 
    }
 }
