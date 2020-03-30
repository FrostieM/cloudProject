import {Inject, Injectable} from "@angular/core";
import {HttpConnection} from "../abstract/httpConnection.abstract";
import {HttpClient} from "@angular/common/http";

@Injectable()
export class UserAuthService extends HttpConnection{

  constructor(public http: HttpClient,
              @Inject("BASE_URL") public baseUrl: string) {
    super(http, baseUrl);
  }

  public login(jsonBody: string){
    return this.postRequest("api/auth/login", jsonBody);
  }

  public registration(jsonBody: string){
    return this.postRequest("api/auth/signUp", jsonBody);
  }
}
