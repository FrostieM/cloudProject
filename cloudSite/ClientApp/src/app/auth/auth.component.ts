import {Component, OnInit} from '@angular/core';
import {AuthService, GoogleLoginProvider} from "angularx-social-login";
import {Router} from "@angular/router";

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['auth.component.css']
})
export class AuthComponent implements OnInit {
  public resultMessage: string;

  constructor(private _authService: AuthService, private _route: Router ) { }

  ngOnInit() {

  }

  logInWithGoogle(): void {
    let platform = GoogleLoginProvider.PROVIDER_ID;

    this._authService.signIn(platform).then(
      response => {

        console.log(platform + ' logged in user data is= ' , response);

        let userData = {
          UserId: response.id,
          Provider: response.provider,
          FirstName: response.firstName,
          LastName: response.lastName,
          EmailAddress: response.email,
          PictureUrl: response.photoUrl
        };

        localStorage.setItem("user", JSON.stringify(userData));
        this._route.navigate([""]).then();
      },
      error => {
        console.log(error);
        this.resultMessage = error;
      })
  }

}
