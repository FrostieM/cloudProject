import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import {SidebarComponent} from "./sidebar/sidebar.component";
import {ComputerService} from "./shared/services/computer.service";

import { SocialLoginModule, AuthServiceConfig } from 'angularx-social-login';
import { GoogleLoginProvider } from 'angularx-social-login';
import {ComputerListComponent} from "./computerList/computerList.component";
import {ProgramListComponent} from "./programList/programList.component";
import {AuthGuard} from "./shared/guards/auth.guard";
import {JwtHelperService, JwtModule} from "@auth0/angular-jwt";
import {TokenService} from "./shared/services/token.service";
import {AuthModule} from "./authentication/auth.module";
import {UserLogInfoComponent} from "./userLogInfo/userLogInfo.component";
import {UserLogInfoService} from "./shared/services/user-log-info.service";

let config = new AuthServiceConfig([
  {
    id: GoogleLoginProvider.PROVIDER_ID,
    provider: new GoogleLoginProvider("339482668525-f218v14289blu55hvhccm6btb2re5c48.apps.googleusercontent.com")
}]);

export function provideConfig()
{
  return config;
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    SidebarComponent,
    ComputerListComponent,
    ProgramListComponent,
    UserLogInfoComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    SocialLoginModule.initialize(config),
    AuthModule,
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthGuard]},
      { path: 'programList/:compName', component: ProgramListComponent, canActivate: [AuthGuard]},
      { path: 'Log', component: UserLogInfoComponent, canActivate: [AuthGuard]},
    ]),
    JwtModule.forRoot({
      config: {
        tokenGetter: tokenGetter,
        whitelistedDomains: ["localhost:5001"],
        blacklistedRoutes: []
      }
    })
  ],
  providers: [
    ComputerService, AuthGuard, JwtHelperService, UserLogInfoService,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }

export function tokenGetter() {
  return new TokenService().Token;
}
