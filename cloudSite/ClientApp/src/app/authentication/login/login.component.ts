import {Component, Inject} from '@angular/core';
import { Router } from "@angular/router";
import {FormControl, FormGroup, Validators} from '@angular/forms';
import {TokenService} from "../../shared/services/token.service";
import {UserAuthService} from "../../shared/services/auth-user.service";
import {AuthService, GoogleLoginProvider} from "angularx-social-login";


@Component({
  selector: 'auth-login-component',
  host: {
    class: "row col-12 m-0 p-0"
  },
  templateUrl: './login.component.html'
})
export class LoginComponent {
  invalidLogin: boolean = false;
  loginForm: FormGroup = new FormGroup({
    email: new FormControl("", [
      Validators.required, Validators.email
    ]),

    password: new FormControl("", [
      Validators.required, Validators.minLength(6), Validators.maxLength(20)
    ])
  });

  constructor(private router: Router,
              @Inject("BASE_URL") private baseUrl: string,
              private _tokenService: TokenService,
              private _userAuthService: UserAuthService,
              private _route: Router) {}

  public login(){
    let credentials = JSON.stringify(this.loginForm.value);

    this._userAuthService.login(credentials).subscribe(response => {
      this._tokenService.addToken(response.token);
      localStorage.setItem("user", response.user);
      this.invalidLogin = false;
      this.router.navigate(["/"]).then(() => {});
    }, () => {
      this.invalidLogin = true;
    });
  }
  public invalidForm(form: FormGroup){
    return form.invalid && form.touched;
  }

}
