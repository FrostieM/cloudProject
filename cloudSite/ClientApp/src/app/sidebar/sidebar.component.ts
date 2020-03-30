import {Component, OnInit} from '@angular/core';
import {AuthService} from "angularx-social-login";
import {Router} from "@angular/router";
import {TokenService} from "../shared/services/token.service";
import {UserLogInfoService} from "../shared/services/user-log-info.service";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit{
  public user: any;

  constructor(private _authService: AuthService,
              private _router: Router,
              private _token: TokenService,
              private _userLogInfoService: UserLogInfoService) {
  }

  public logOut(): void {
    this._authService.signOut().then();
    this._token.removeToken();
    localStorage.removeItem("user");
    this._router.navigate(["auth"]).then();
  }

  ngOnInit(): void {
    this.user = localStorage.getItem("user");
  }



}
