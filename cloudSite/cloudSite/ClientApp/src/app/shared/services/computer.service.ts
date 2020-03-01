import {Inject, Injectable} from "@angular/core";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {IComputer} from "../interfaces/computer.interface";

@Injectable()
export class ComputerService{
  constructor(private http: HttpClient,
              @Inject("BASE_URL") private baseUrl: string) {
  }

  public getComputers(): Observable<IComputer[]>{
    return this.getRequest<IComputer[]>(this.baseUrl + "api/computer");
  }

  public getComputerInfo(computerName: string): Observable<string[]>{
    return this.getRequest<string[]>(this.baseUrl + "api/computer/computerInfo");
  }

  private getRequest<T>(url: string, params?: HttpParams, responseType?): Observable<T>{
    return this.http.get<T>(url, {
      headers: new HttpHeaders({
        "Content-Type": "application/json"
      }),
      params: params,
      responseType: responseType
    });
  }

}
