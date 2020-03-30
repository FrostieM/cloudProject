import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Inject} from "@angular/core";
import {Observable} from "rxjs";

export abstract class HttpConnection{

  protected constructor(public http: HttpClient,
              @Inject("BASE_URL") public baseUrl: string) {
  }

  protected postRequest(url: string, jsonBody: string){
    return this.http.post<any>(this.baseUrl + url, jsonBody, {
      headers: new HttpHeaders({
        "Content-Type": "application/json"
      })
    });
  }

  protected getRequest<T>(url: string, params?: HttpParams, responseType?): Observable<T>{
    return this.http.get<T>(this.baseUrl + url, {
      headers: new HttpHeaders({
        "Content-Type": "application/json"
      }),
      params: params,
      responseType: responseType
    });
  }
}
