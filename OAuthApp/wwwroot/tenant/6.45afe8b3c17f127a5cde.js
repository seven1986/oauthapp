(window.webpackJsonp=window.webpackJsonp||[]).push([[6],{"8igp":function(e,t,i){"use strict";i.r(t),i.d(t,"UserModule",function(){return J});var n=i("ofXK"),c=i("tyNb"),o=i("fXoL"),a=i("8lDW");let b=(()=>{class e{constructor(e,t){this.authService=e,this.router=t}ngOnInit(){}logout(){this.authService.logout(),this.router.navigate(["/"])}}return e.\u0275fac=function(t){return new(t||e)(o.Ib(a.a),o.Ib(c.f))},e.\u0275cmp=o.Cb({type:e,selectors:[["app-user"]],decls:4,vars:0,consts:[[1,"container-fluid"],[1,"row"],[1,"col"]],template:function(e,t){1&e&&(o.Nb(0,"div",0),o.Nb(1,"div",1),o.Nb(2,"div",2),o.Jb(3,"router-outlet"),o.Mb(),o.Mb(),o.Mb())},directives:[c.j],styles:[""]}),e})();var r=i("AytR"),s=i("JqCM"),l=i("5eHb"),d=i("b6Qw"),u=i("3Pt+");const p=function(e,t){return{"is-valid":e,"is-invalid":t}};let f=(()=>{class e{constructor(e,t,i,n){this.authService=e,this.spinner=t,this.toast=i,this.cookie=n,this.pageModel={mobile:""}}ngOnInit(){this.pageModel.mobile=this.authService.user.phone_number}updatePhoneNumber(){this.spinner.show(),this.authService.changePhoneNumber(this.pageModel.mobile).subscribe(e=>{this.spinner.hide(),200==e.code&&(this.toast.success("\u4fee\u6539\u6210\u529f\uff01"),this.authService.user.phone_number=this.pageModel.mobile,this.authService.loginByAccesstoken(this.cookie.get(r.a.identity.token_cookies_key)))},e=>{this.spinner.hide()})}}return e.\u0275fac=function(t){return new(t||e)(o.Ib(a.a),o.Ib(s.c),o.Ib(l.b),o.Ib(d.a))},e.\u0275cmp=o.Cb({type:e,selectors:[["app-profile"]],decls:22,vars:8,consts:function(){let e,t,i,n,c;return e=$localize`:␟c57581f52101eb9c0f2af4ca6e8c08a10c4650cc␟4030732306132103411:基础信息`,t=$localize`:␟0115e0ed1b079ec138ed51b29c8e203921fb272b␟6363998355026114303:账号`,i=$localize`:␟67a594abe6c9ec288df6f60cd8802ba00c82f8b8␟7599848388841641056:手机号`,n=$localize`:␟27df5cba345c2681b45620d6b7d0af172edb66a9␟7155530627389289799:mobile is required`,c=$localize`:␟746769f9e6fd13ab3afe6341c247fd31d8449ab3␟5399534098099072067: 更新 `,[[1,"p-3"],e,["pageForm","ngForm"],[1,"form-group"],["for","username"],t,["for","mobile"],i,["id","mobile","type","text","name","mobile","required","",1,"form-control",3,"ngClass","ngModel","ngModelChange"],["mobile","ngModel"],[1,"invalid-feedback",3,"hidden"],n,[1,"d-flex"],[1,"flex-fill"],[2,"position","relative"],["type","button",1,"btn","btn-primary","btn-block","mt-3",3,"disabled","click"],c]},template:function(e,t){if(1&e&&(o.Nb(0,"div",0),o.Nb(1,"h5"),o.Rb(2,1),o.Mb(),o.Jb(3,"hr"),o.Nb(4,"form",null,2),o.Nb(6,"div",3),o.Nb(7,"label",4),o.Rb(8,5),o.Mb(),o.uc(9),o.Mb(),o.Nb(10,"div",3),o.Nb(11,"label",6),o.Rb(12,7),o.Mb(),o.Nb(13,"input",8,9),o.Xb("ngModelChange",function(e){return t.pageModel.mobile=e}),o.Mb(),o.Nb(15,"div",10),o.Rb(16,11),o.Mb(),o.Mb(),o.Nb(17,"section",12),o.Nb(18,"div",13),o.Nb(19,"div",14),o.Nb(20,"button",15),o.Xb("click",function(){return t.updatePhoneNumber()}),o.Rb(21,16),o.Mb(),o.Mb(),o.Mb(),o.Mb(),o.Mb(),o.Mb()),2&e){const e=o.lc(5),i=o.lc(14);o.xb(9),o.wc(" ",null==t.authService.user?null:t.authService.user.name," "),o.xb(4),o.gc("ngClass",o.jc(5,p,i.valid,i.invalid&&i.dirty))("ngModel",t.pageModel.mobile),o.xb(2),o.gc("hidden",i.valid),o.xb(5),o.gc("disabled",!e.form.valid)}},directives:[u.h,u.d,u.e,u.a,u.g,n.i,u.c,u.f],styles:[""]}),e})();const h=function(e,t){return{"is-valid":e,"is-invalid":t}};let g=(()=>{class e{constructor(e,t,i,n){this.authService=e,this.spinner=t,this.toast=i,this.cookie=n,this.pageModel={email:""}}updateEmail(){this.spinner.show(),this.authService.ChangeEmail(this.pageModel.email).subscribe(e=>{this.spinner.hide(),200==e.code&&(this.toast.success("\u5df2\u53d1\u9001\u9a8c\u8bc1\u4fe1\u606f\uff0c\u8bf7\u524d\u5f80\u90ae\u7bb1\u8fdb\u884c\u9a8c\u8bc1"),this.authService.user.email=this.pageModel.email,this.authService.loginByAccesstoken(this.cookie.get(r.a.identity.token_cookies_key)))},e=>{this.spinner.hide()})}}return e.\u0275fac=function(t){return new(t||e)(o.Ib(a.a),o.Ib(s.c),o.Ib(l.b),o.Ib(d.a))},e.\u0275cmp=o.Cb({type:e,selectors:[["app-email"]],decls:22,vars:8,consts:function(){let e,t,i,n,c;return e=$localize`:␟335324a49dc2a2736a482c8dc6a9205d0e6b338c␟9115505226015911600:邮箱`,t=$localize`:␟335324a49dc2a2736a482c8dc6a9205d0e6b338c␟9115505226015911600:邮箱`,i=$localize`:␟64630ac10305bb532915293a807a1e4dca5ad5e3␟7825794376019502119:新的邮箱`,n=$localize`:␟add7e6e8e77f78fbd169b6cd5333d94e80036da6␟8238982058209538184:email is required`,c=$localize`:␟875609cb7cd36be2ca2765fec5510c057a575b95␟7140362554388861586: 更新 `,[[1,"p-3"],e,["pageForm","ngForm"],[1,"form-group","text-break"],["for","username"],t,["for","email"],i,["id","email","type","text","name","email","required","",1,"form-control",3,"ngClass","ngModel","ngModelChange"],["email","ngModel"],[1,"invalid-feedback",3,"hidden"],n,[1,"d-flex"],[1,"flex-fill"],[2,"position","relative"],["type","button",1,"btn","btn-primary","btn-block","mt-3",3,"disabled","click"],c]},template:function(e,t){if(1&e&&(o.Nb(0,"div",0),o.Nb(1,"h5"),o.Rb(2,1),o.Mb(),o.Jb(3,"hr"),o.Nb(4,"form",null,2),o.Nb(6,"div",3),o.Nb(7,"label",4),o.Rb(8,5),o.Mb(),o.uc(9),o.Mb(),o.Nb(10,"div",3),o.Nb(11,"label",6),o.Rb(12,7),o.Mb(),o.Nb(13,"input",8,9),o.Xb("ngModelChange",function(e){return t.pageModel.email=e}),o.Mb(),o.Nb(15,"div",10),o.Rb(16,11),o.Mb(),o.Mb(),o.Nb(17,"section",12),o.Nb(18,"div",13),o.Nb(19,"div",14),o.Nb(20,"button",15),o.Xb("click",function(){return t.updateEmail()}),o.Rb(21,16),o.Mb(),o.Mb(),o.Mb(),o.Mb(),o.Mb(),o.Mb()),2&e){const e=o.lc(5),i=o.lc(14);o.xb(9),o.wc(" ",t.authService.user.email," "),o.xb(4),o.gc("ngClass",o.jc(5,h,i.valid,i.invalid&&i.dirty))("ngModel",t.pageModel.email),o.xb(2),o.gc("hidden",i.valid),o.xb(5),o.gc("disabled",!e.form.valid)}},directives:[u.h,u.d,u.e,u.a,u.g,n.i,u.c,u.f],styles:[""]}),e})();const m=function(e,t){return{"is-valid":e,"is-invalid":t}};let v=(()=>{class e{constructor(e,t,i){this.authService=e,this.spinner=t,this.toast=i,this.pageModel={oldpwd:"",newpwd:"",newpwd2:""}}updatePassword(){this.spinner.show(),this.authService.changePassword(this.pageModel.oldpwd,this.pageModel.newpwd,this.pageModel.newpwd2).subscribe(e=>{this.spinner.hide(),200==e.code?this.toast.success("\u4fee\u6539\u6210\u529f\uff01"):this.toast.error(e.message,e.codeName)},e=>{this.spinner.hide()})}}return e.\u0275fac=function(t){return new(t||e)(o.Ib(a.a),o.Ib(s.c),o.Ib(l.b))},e.\u0275cmp=o.Cb({type:e,selectors:[["app-change-password"]],decls:32,vars:19,consts:function(){let e,t,i,n,c,o,a,b;return e=$localize`:␟d3b7cf51fce8e4f908e2a834a9d8d1ca5f4faa3e␟4619345994489495392:修改密码`,t=$localize`:␟b727cef2d734f7ad790884a923a7d6bf08b7b002␟4338088824747197346:当前密码`,i=$localize`:␟c8d33a1700b5b8c997d619acaca34a0e5e757e89␟8872271492605446692:oldpwd is required`,n=$localize`:␟0745a6e4f3c14eedb5060a46b3b5752231498031␟6069854734457630856:新的密码`,c=$localize`:␟748f8bd03fa5450c6217b0c414bcf2558ea88c52␟4928776968054380826:newpwd is required`,o=$localize`:␟3324f7c94dfd4042c094d5457f761f52d7aadca5␟7391278566336471837:再次输入`,a=$localize`:␟eb532d051e9854f58ade03cd7660f6264d3b9020␟1507974293631459691:newpwd2 is required`,b=$localize`:␟746769f9e6fd13ab3afe6341c247fd31d8449ab3␟5399534098099072067: 更新 `,[[1,"p-3"],e,["pageForm","ngForm"],[1,"form-group"],["for","oldpwd"],t,["id","oldpwd","type","text","name","oldpwd","required","",1,"form-control",3,"ngClass","ngModel","ngModelChange"],["oldpwd","ngModel"],[1,"invalid-feedback",3,"hidden"],i,["for","newpwd"],n,["id","newpwd","type","text","name","newpwd","required","",1,"form-control",3,"ngClass","ngModel","ngModelChange"],["newpwd","ngModel"],c,["for","newpwd2"],o,["id","newpwd2","type","text","name","newpwd2","required","",1,"form-control",3,"ngClass","ngModel","ngModelChange"],["newpwd2","ngModel"],a,[1,"d-flex"],[1,"flex-fill"],[2,"position","relative"],["type","button",1,"btn","btn-primary","btn-block","mt-3",3,"disabled","click"],b]},template:function(e,t){if(1&e&&(o.Nb(0,"div",0),o.Nb(1,"h5"),o.Rb(2,1),o.Mb(),o.Jb(3,"hr"),o.Nb(4,"form",null,2),o.Nb(6,"div",3),o.Nb(7,"label",4),o.Rb(8,5),o.Mb(),o.Nb(9,"input",6,7),o.Xb("ngModelChange",function(e){return t.pageModel.oldpwd=e}),o.Mb(),o.Nb(11,"div",8),o.Rb(12,9),o.Mb(),o.Mb(),o.Nb(13,"div",3),o.Nb(14,"label",10),o.Rb(15,11),o.Mb(),o.Nb(16,"input",12,13),o.Xb("ngModelChange",function(e){return t.pageModel.newpwd=e}),o.Mb(),o.Nb(18,"div",8),o.Rb(19,14),o.Mb(),o.Mb(),o.Nb(20,"div",3),o.Nb(21,"label",15),o.Rb(22,16),o.Mb(),o.Nb(23,"input",17,18),o.Xb("ngModelChange",function(e){return t.pageModel.newpwd2=e}),o.Mb(),o.Nb(25,"div",8),o.Rb(26,19),o.Mb(),o.Mb(),o.Nb(27,"section",20),o.Nb(28,"div",21),o.Nb(29,"div",22),o.Nb(30,"button",23),o.Xb("click",function(){return t.updatePassword()}),o.Rb(31,24),o.Mb(),o.Mb(),o.Mb(),o.Mb(),o.Mb(),o.Mb()),2&e){const e=o.lc(5),i=o.lc(10),n=o.lc(17),c=o.lc(24);o.xb(9),o.gc("ngClass",o.jc(10,m,i.valid,i.invalid&&i.dirty))("ngModel",t.pageModel.oldpwd),o.xb(2),o.gc("hidden",i.valid),o.xb(5),o.gc("ngClass",o.jc(13,m,n.valid,n.invalid&&n.dirty))("ngModel",t.pageModel.newpwd),o.xb(2),o.gc("hidden",n.valid),o.xb(5),o.gc("ngClass",o.jc(16,m,c.valid,c.invalid&&c.dirty))("ngModel",t.pageModel.newpwd2),o.xb(2),o.gc("hidden",c.valid),o.xb(5),o.gc("disabled",!e.form.valid)}},directives:[u.h,u.d,u.e,u.a,u.g,n.i,u.c,u.f],styles:[""]}),e})();function M(e,t){if(1&e){const e=o.Ob();o.Nb(0,"li",5),o.Nb(1,"a",6),o.Xb("click",function(){o.mc(e);const i=t.$implicit;return o.bc(2).showProviderKey(i.providerKey)}),o.uc(2),o.Mb(),o.Nb(3,"a",7),o.Xb("click",function(){o.mc(e);const i=t.$implicit;return o.bc(2).removeExternalLogin(i.loginProvider,i.providerKey)}),o.uc(4," \u5220\u9664 "),o.Mb(),o.Mb()}if(2&e){const e=t.$implicit;o.xb(2),o.wc("",e.loginProvider," ")}}function w(e,t){if(1&e&&(o.Nb(0,"div"),o.Nb(1,"h6"),o.uc(2,"\u5df2\u7ed1\u5b9a\u8d26\u53f7"),o.Mb(),o.Nb(3,"ul",3),o.sc(4,M,5,1,"li",4),o.Mb(),o.Mb()),2&e){const e=o.bc();o.xb(4),o.gc("ngForOf",e.data.logins)}}function N(e,t){if(1&e){const e=o.Ob();o.Nb(0,"button",10),o.Xb("click",function(){o.mc(e);const i=t.$implicit;return o.bc(2).linkExternalLogin(i)}),o.uc(1),o.Mb()}if(2&e){const e=t.$implicit;o.xb(1),o.wc(" ",e," ")}}function y(e,t){if(1&e&&(o.Nb(0,"div"),o.Nb(1,"h6",8),o.uc(2,"\u53ef\u7ed1\u5b9a\u7b2c\u4e09\u65b9\u8d26\u53f7"),o.Mb(),o.sc(3,N,2,1,"button",9),o.Mb()),2&e){const e=o.bc();o.xb(3),o.gc("ngForOf",e.data.otherLogins)}}let x=(()=>{class e{constructor(e,t,i,n,c){this.authSerive=e,this.toast=t,this.spinner=i,this.route=n,this.router=c,this.data=[],this.route.queryParams.subscribe(e=>{if(e.error){var t=JSON.parse(e.error);this.toast.error(t[0].Description,t[0].Code),this.router.navigateByUrl("/user/external-logins")}}),this.getData()}getData(){this.authSerive.MyLogins().subscribe(e=>{this.data=e.data})}linkExternalLogin(e){this.spinner.show();var t=encodeURIComponent(location.href);location.href=`${r.a.identity.server}/Authing/LinkExternalLogin?provider=${e}&redirectUrl=${t}`}removeExternalLogin(e,t){this.authSerive.RemoveExternalLogin(e,t).subscribe(e=>{200!=e.code&&this.toast.error(e.message),this.getData()})}showProviderKey(e){this.toast.success(e,"openid",{positionClass:"toast-center-center",progressBar:!1,closeButton:!1})}}return e.\u0275fac=function(t){return new(t||e)(o.Ib(a.a),o.Ib(l.b),o.Ib(s.c),o.Ib(c.a),o.Ib(c.f))},e.\u0275cmp=o.Cb({type:e,selectors:[["app-external-logins"]],decls:6,vars:2,consts:function(){let e;return e=$localize`:␟687fb8d8f16e237ea9773a37f27ef6115aec8922␟6351014673530979254:第三方登录`,[[1,"p-3"],e,[4,"ngIf"],[1,"list-group","my-4"],["class","list-group-item d-flex justify-content-between align-items-center",4,"ngFor","ngForOf"],[1,"list-group-item","d-flex","justify-content-between","align-items-center"],["href","javascript:void(0)",3,"click"],["href","javascript:void(0)",1,"btn","btn-primary","btn-sm",3,"click"],[1,"mb-4"],["class","btn btn-light btn-block border mb-3",3,"click",4,"ngFor","ngForOf"],[1,"btn","btn-light","btn-block","border","mb-3",3,"click"]]},template:function(e,t){1&e&&(o.Nb(0,"div",0),o.Nb(1,"h5"),o.Rb(2,1),o.Mb(),o.Jb(3,"hr"),o.sc(4,w,5,1,"div",2),o.sc(5,y,4,1,"div",2),o.Mb()),2&e&&(o.xb(4),o.gc("ngIf",t.data.logins&&t.data.logins.length>0),o.xb(1),o.gc("ngIf",t.data.otherLogins&&t.data.otherLogins.length>0))},directives:[n.k,n.j],styles:[""]}),e})(),k=(()=>{class e{constructor(){}ngOnInit(){}}return e.\u0275fac=function(t){return new(t||e)},e.\u0275cmp=o.Cb({type:e,selectors:[["app-two-factor-authentication"]],decls:6,vars:0,consts:function(){let e;return e=$localize`:␟a2cc7972c80a5afee0451a8304ae8e297378a9ee␟3730157676830583758:双因素认证`,[[1,"p-3"],e,["pageForm","ngForm"]]},template:function(e,t){1&e&&(o.Nb(0,"div",0),o.Nb(1,"h5"),o.Rb(2,1),o.Mb(),o.Jb(3,"hr"),o.Jb(4,"form",null,2),o.Mb())},directives:[u.h,u.d,u.e],styles:[""]}),e})(),C=(()=>{class e{constructor(){}ngOnInit(){}}return e.\u0275fac=function(t){return new(t||e)},e.\u0275cmp=o.Cb({type:e,selectors:[["app-personal-data"]],decls:6,vars:0,consts:function(){let e;return e=$localize`:␟3413cde09bc192be2326f099d704c2762cac062d␟6987028809894157110:注销账户`,[[1,"p-3"],e,["pageForm","ngForm"]]},template:function(e,t){1&e&&(o.Nb(0,"div",0),o.Nb(1,"h5"),o.Rb(2,1),o.Mb(),o.Jb(3,"hr"),o.Jb(4,"form",null,2),o.Mb())},directives:[u.h,u.d,u.e],styles:[""]}),e})();var $=i("xfLO");function I(e,t){if(1&e&&(o.Nb(0,"span",12),o.uc(1),o.Mb()),2&e){const e=t.$implicit;o.xb(1),o.wc(" ",e," ")}}function R(e,t){if(1&e&&(o.Nb(0,"p"),o.uc(1),o.Mb()),2&e){const e=o.bc().$implicit;o.xb(1),o.wc(" ",e.description," ")}}function z(e,t){if(1&e&&(o.Nb(0,"small"),o.uc(1),o.Mb()),2&e){const e=o.bc().$implicit;o.xb(1),o.wc("\uff0c\u5230\u671f\u65f6\u95f4\uff1a",e.expiration,"")}}function F(e,t){if(1&e){const e=o.Ob();o.Nb(0,"div",5),o.Nb(1,"div",6),o.Nb(2,"h6"),o.Nb(3,"button",7),o.Xb("click",function(){o.mc(e);const i=t.$implicit;return o.bc().revokeConsent(i.clientId)}),o.uc(4,"\u64a4\u9500"),o.Mb(),o.uc(5),o.Mb(),o.Mb(),o.Nb(6,"div",8),o.sc(7,I,2,1,"span",9),o.sc(8,R,2,1,"p",10),o.Mb(),o.Nb(9,"div",11),o.Nb(10,"small"),o.uc(11),o.Mb(),o.sc(12,z,2,1,"small",10),o.Mb(),o.Mb()}if(2&e){const e=t.$implicit;o.xb(5),o.wc(" ",e.clientId," "),o.xb(2),o.gc("ngForOf",e.scopes),o.xb(1),o.gc("ngIf",e.description),o.xb(3),o.wc("\u6388\u6743\u65f6\u95f4\uff1a",e.creationTime,""),o.xb(1),o.gc("ngIf",e.expiration)}}const S=[{path:"",component:b,children:[{path:"",redirectTo:"profile",pathMatch:"full"},{path:"profile",component:f},{path:"email",component:g},{path:"change-password",component:v},{path:"external-logins",component:x},{path:"two-factor-authentication",component:k},{path:"grants",component:(()=>{class e{constructor(e,t,i){this.oauthapp=e,this.spinner=t,this.toastr=i,this.data=[],this.getData()}getData(){this.spinner.show(),this.oauthapp.Grants().subscribe(e=>{this.spinner.hide(),this.data=e})}revokeConsent(e){this.spinner.show(),this.oauthapp.Revoke(e).subscribe(e=>{this.spinner.hide(),this.getData()})}}return e.\u0275fac=function(t){return new(t||e)(o.Ib($.a),o.Ib(s.c),o.Ib(l.b))},e.\u0275cmp=o.Cb({type:e,selectors:[["app-grants"]],decls:7,vars:1,consts:function(){let e;return e=$localize`:␟df6068d4c48f19c1752683c5e6b7c950264c9fe8␟2633075000214992170:授权管理`,[["size","default","color","#fff","type","ball-beat"],[1,"p-3"],e,[1,"mt-4"],["class","card my-3",4,"ngFor","ngForOf"],[1,"card","my-3"],[1,"card-header"],["type","button",1,"btn","btn-danger","btn-sm","float-right",3,"click"],[1,"card-body"],["class","badge badge-primary mx-1 my-2","style","font-size: 100%;",4,"ngFor","ngForOf"],[4,"ngIf"],[1,"card-footer","text-muted","text-right"],[1,"badge","badge-primary","mx-1","my-2",2,"font-size","100%"]]},template:function(e,t){1&e&&(o.Jb(0,"ngx-spinner",0),o.Nb(1,"div",1),o.Nb(2,"h5"),o.Rb(3,2),o.Mb(),o.Jb(4,"hr"),o.Nb(5,"div",3),o.sc(6,F,13,5,"div",4),o.Mb(),o.Mb()),2&e&&(o.xb(6),o.gc("ngForOf",t.data))},directives:[s.a,n.j,n.k],styles:[""]}),e})()},{path:"personal-data",component:C}]}];let O=(()=>{class e{}return e.\u0275mod=o.Gb({type:e}),e.\u0275inj=o.Fb({factory:function(t){return new(t||e)},imports:[[c.i.forChild(S)],c.i]}),e})(),J=(()=>{class e{}return e.\u0275mod=o.Gb({type:e}),e.\u0275inj=o.Fb({factory:function(t){return new(t||e)},imports:[[n.b,u.b,O,s.b]]}),e})()}}]);