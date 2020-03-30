import {Component, Inject} from '@angular/core';
import { Router } from "@angular/router";
import { FormBuilder, FormControl, FormGroup, Validators} from '@angular/forms';
import {TokenService} from "../../shared/services/token.service";
import {UserAuthService} from "../../shared/services/auth-user.service";

@Component({
  selector: 'auth-registration-component',
  host: {
    class: "row col-12 m-0 p-0"
  },
  templateUrl: './registration.component.html'
})
export class RegistrationComponent {
  invalidSignUp: boolean = false;
  signUpForm: FormGroup;

  constructor(private router: Router,
              private fb: FormBuilder,
              @Inject("BASE_URL") private baseUrl: string,
              private _tokenService: TokenService,
              private _userAuthService: UserAuthService,
              private _route: Router) {

    this.signUpForm = fb.group({
      email: new FormControl( "", [
        Validators.required, Validators.email
      ]),

      password: new FormControl("", [
        Validators.required, Validators.minLength(6), Validators.maxLength(20)
      ]),

      rePassword: new FormControl("", ),

      firstname: new FormControl("", [
        Validators.required,
      ]),

      surname: new FormControl("", [
        Validators.required,
      ]),

    }, {
      validators: this.checkPasswords
    });
  }

  public registration(){
    let credentials = JSON.stringify(this.signUpForm.value);

    this._userAuthService.registration(credentials).subscribe(response => {
      this._tokenService.addToken(response.token);
      localStorage.setItem("user", response.user);
      this.invalidSignUp = false;
      this.router.navigate(["/"]).then(() => {});
    }, () => {
      this.invalidSignUp = true;
    });
  }

  public invalidForm(form: FormGroup){
    return form.invalid && form.touched;
  }

  checkPasswords(form: FormGroup) { // here we have the 'passwords' group
    let pass = form.get('password').value;
    let confirmPass = form.get('rePassword').value;

    return pass === confirmPass ? null : { notSame: true }
  }

}
