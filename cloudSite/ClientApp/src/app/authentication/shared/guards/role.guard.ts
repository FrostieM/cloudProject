import {Injectable} from "@angular/core";

import { CanActivate, Router } from '@angular/router';
import { JwtHelperService } from '@auth0/angular-jwt';
import {TokenService} from "../../../shared/services/token.service";



@Injectable()
export class RoleGuard implements CanActivate {
  constructor(private jwtHelper: JwtHelperService,
              private router: Router,
              private tokenService: TokenService) {}
  canActivate() {

    if (!this.tokenService.IsTokenCorrect){
      return true;
    }

    this.router.navigate([""]).then(() => {});
    return false;
  }
}
