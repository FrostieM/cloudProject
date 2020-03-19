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
import {AuthComponent} from "./auth/auth.component";
import {ComputerListComponent} from "./computerList/computerList.component";
import {ProgramListComponent} from "./programList/programList.component";
import {AuthGuard} from "./shared/guards/auth.guard";

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
    AuthComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    SocialLoginModule.initialize(config),
    RouterModule.forRoot([
      { path: '', component: HomeComponent, pathMatch: 'full', canActivate: [AuthGuard]},
      { path: 'auth', component: AuthComponent},
      { path: 'programList/:compName', component: ProgramListComponent, canActivate: [AuthGuard]},
    ])
  ],
  providers: [
    ComputerService, AuthGuard
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
