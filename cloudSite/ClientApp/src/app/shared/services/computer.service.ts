import {Inject, Injectable} from "@angular/core";
import {HttpClient, HttpHeaders, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {IComputer} from "../interfaces/computer.interface";
import {IComputerViewData} from "../interfaces/computerViewData.interface";

@Injectable()
export class ComputerService{
  constructor(private http: HttpClient,
              @Inject("BASE_URL") private baseUrl: string) {
  }

  public getComputers(): Observable<IComputer[]>{
    return this.getRequest<IComputer[]>(this.baseUrl + "api/computer");
  }

  public getComputersByProgram(program: string): Observable<IComputer[]>{
    let params = new HttpParams().set("programName", program);
    return this.getRequest<IComputer[]>(this.baseUrl + "api/computer/getCompsByProgram", params);
  }

  public getComputerInfo(computerName: string, page: number): Observable<IComputerViewData>{
    let params = new HttpParams().set("computerName", computerName).set("page", page.toString());
    return this.getRequest<IComputerViewData>(this.baseUrl + "api/computer/computerInfo", params);
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
