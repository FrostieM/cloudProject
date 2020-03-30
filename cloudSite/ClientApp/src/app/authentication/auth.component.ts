import {Component, Inject, OnInit} from '@angular/core';
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {TokenService} from "../shared/services/token.service";

@Component({
  selector: 'app-auth-component',
  templateUrl: './auth.component.html',
  styleUrls: ['auth.component.css'],
})
export class UserAuthComponent implements OnInit{

  isLoginForm: boolean = true;

  constructor(private _router: Router,
              private http: HttpClient,
              @Inject("BASE_URL") private baseUrl: string) {}

  ngOnInit(): void {
    //if page was update when was signUp
    this.isLoginForm = this._router.url.split("/").pop() != "signUp";
    console.log(new TokenService().Token);
  }
}
