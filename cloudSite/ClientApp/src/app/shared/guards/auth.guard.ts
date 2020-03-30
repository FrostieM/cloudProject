import {Injectable} from "@angular/core";
import {CanActivate, Router} from "@angular/router";
import {TokenService} from "../services/token.service";
import {JwtHelperService} from "@auth0/angular-jwt";

@Injectable()
export class AuthGuard implements CanActivate {
  constructor(private jwtHelper: JwtHelperService,
              private router: Router,
              private tokenService: TokenService) {}

  canActivate() {
    if (this.tokenService.IsTokenCorrect){
      return true;
    }

    this.router.navigate(["auth"]).then(() => {});
    return false;
  }
}
