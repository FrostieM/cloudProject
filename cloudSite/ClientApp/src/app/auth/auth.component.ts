import {Component, OnInit} from '@angular/core';
import {ComputerService} from "../shared/services/computer.service";
import {AuthService, GoogleLoginProvider} from "angularx-social-login";

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['auth.component.css']
})
export class AuthComponent implements OnInit {
  public userData: any;
  public resultMessage: string;

  constructor( private authService: AuthService ) { }

  ngOnInit() {

  }

  logInWithGoogle(): void {
    let platform = GoogleLoginProvider.PROVIDER_ID;

    this.authService.signIn(platform).then(
      response => {

        console.log(platform + ' logged in user data is= ' , response);

        this.userData = {
          UserId: response.id,
          Provider: response.provider,
          FirstName: response.firstName,
          LastName: response.lastName,
          EmailAddress: response.email,
          PictureUrl: response.photoUrl
        };
      },
      error => {
        console.log(error);
        this.resultMessage = error;
      })
  }

  public logOut(): void {
    this.authService.signOut().then();
    this.userData = null;
    console.log('User has signed our');
  }

}
