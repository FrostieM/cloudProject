import {JwtHelperService} from "@auth0/angular-jwt";
import {Injectable} from "@angular/core";

@Injectable()
export class TokenService {

  public get Token(){
    return localStorage.getItem(this._tokenName);
  }

  public get IsTokenCorrect(): boolean{
    return this.Token && !this._jwt.isTokenExpired(this.Token);
  }

  public addToken(token: string): void{
    localStorage.setItem(this._tokenName, token)
  }

  public removeToken(): void{
    localStorage.removeItem(this._tokenName)
  }

  private readonly _tokenName: string = "token";
  private _jwt = new JwtHelperService();

  constructor() {}

}
