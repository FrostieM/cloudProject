import {Inject, Injectable} from "@angular/core";
import {HttpConnection} from "../abstract/httpConnection.abstract";
import {Observable} from "rxjs";
import {UserLogInfo} from "../interfaces/userLogInfo.interface";
import {HttpClient} from "@angular/common/http";
import {User} from "../interfaces/user.interface";

@Injectable()
export class UserLogInfoService extends HttpConnection{

  constructor(public http: HttpClient,
              @Inject("BASE_URL") public baseUrl: string) {
    super(http, baseUrl);
  }

  public getUser(): Observable<User>{
    return this.getRequest<User>("api/userInfo/getUser");
  }

  public getUsersLogInfo(): Observable<UserLogInfo[]>{
    return this.getRequest<UserLogInfo[]>("api/userInfo/getUsersLogInfo");
  }

}
