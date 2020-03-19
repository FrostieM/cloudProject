import {Injectable} from "@angular/core";
import {CanActivate, Router} from "@angular/router";

@Injectable()
export class AuthGuard implements CanActivate{
  constructor(private _route: Router) {
  }

  canActivate(): boolean {
    let user = localStorage.getItem("user");

    if (user == null){
      this._route.navigate(['auth']).then();
      return false;
    }

    return true;
  }

}
