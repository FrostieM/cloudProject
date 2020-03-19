import {Component, NgZone} from '@angular/core';
import {ComputerService} from "../shared/services/computer.service";
import {AuthService} from "angularx-social-login";
import {Router} from "@angular/router";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent {

  private _user: any;

  constructor(private _authService: AuthService, private _router: Router) {
    this._user = JSON.parse(localStorage.getItem("user"));
  }

  public logOut(): void {
    this._authService.signOut().then();
    localStorage.removeItem("user");
    this._router.navigate(["auth"]).then();
  }



}
