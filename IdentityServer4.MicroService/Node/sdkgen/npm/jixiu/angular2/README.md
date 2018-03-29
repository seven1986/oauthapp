## jixiu.identityserver

### Install
```
npm install jixiu.identityserver --save
```


#### In your angular4 project:

```typescript
// app.module.ts
import { IdentityServerClient } from 'jixiu.identityserver';
@NgModule({
    providers: [ IdentityServerClient ],
})
export class AppModule { }


// demo.ts
import { Component, OnInit } from '@angular/core';
import { IdentityServerClient } from 'jixiu.identityserver';
@Component({
  selector: 'demo',
})
export class DemoComponent implements OnInit {

  constructor(private ids:IdentityServerClient) { }

  ngOnInit() {
    //this.ids.apiresource_get()
  }
}
```  