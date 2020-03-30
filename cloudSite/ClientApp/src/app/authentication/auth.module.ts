import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import {CommonModule} from "@angular/common";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {LoginComponent} from "./login/login.component";
import {RegistrationComponent} from "./registration/registration.component";
import {UserAuthComponent} from "./auth.component";
import {AuthService} from "angularx-social-login";
import {UserAuthService} from "../shared/services/auth-user.service";
import {TokenService} from "../shared/services/token.service";
import {RoleGuard} from "./shared/guards/role.guard";


@NgModule({
  declarations: [
    UserAuthComponent,
    LoginComponent,
    RegistrationComponent
  ],
    imports: [
      CommonModule,
      FormsModule,
      ReactiveFormsModule,
      RouterModule.forRoot([
        {
          path: 'auth', component: UserAuthComponent, children: [
            {path: '', component: LoginComponent},
            {path: 'signUp', component: RegistrationComponent}]
        },
      ])
    ],
  providers: [AuthService, UserAuthService, TokenService],
  exports: []
})
export class AuthModule { }

