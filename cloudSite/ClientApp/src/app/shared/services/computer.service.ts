import {Inject, Injectable} from "@angular/core";
import {HttpClient, HttpParams} from "@angular/common/http";
import {Observable} from "rxjs";
import {IComputer} from "../interfaces/computer.interface";
import {IComputerViewData} from "../interfaces/computerViewData.interface";
import {HttpConnection} from "../abstract/httpConnection.abstract";

@Injectable()
export class ComputerService extends HttpConnection{

  constructor(public http: HttpClient,
              @Inject("BASE_URL") public baseUrl: string) {
    super(http, baseUrl);
  }

  public getComputers(): Observable<IComputer[]>{
    return this.getRequest<IComputer[]>("api/computer");
  }

  public getComputersByProgram(program: string): Observable<IComputer[]>{
    let params = new HttpParams().set("programName", program);
    return this.getRequest<IComputer[]>("api/computer/getCompsByProgram", params);
  }

  public getComputerInfo(computerName: string, page: number): Observable<IComputerViewData>{
    let params = new HttpParams().set("computerName", computerName).set("page", page.toString());
    return this.getRequest<IComputerViewData>("api/computer/computerInfo", params);
  }

}
