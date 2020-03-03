import { Component } from '@angular/core';
import {ComputerService} from "../shared/services/computer.service";
import {IComputerInfo} from "../shared/interfaces/computerInfo.interface";
import {IComputerViewData} from "../shared/interfaces/computerViewData.interface";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['home.component.css']
})
export class HomeComponent {

  public computerViewData: IComputerViewData;

  private _computer: string;
  public set Computer(computer: string){
    this._computer = computer;
    this.computerViewData = null;
    this.getComputerInfo(this._computer, 1);
  }

  public get Computer(){
    return this._computer;
  }

  constructor(private computerService: ComputerService) {
  }

  public getComputerInfo(computerName: string, page: number){
    this.computerService.getComputerInfo(this.Computer, page).subscribe(
      request => this.computerViewData = request
    );
  }

  public getPages(){
    if(!this.computerViewData)
      return null;

    let pages = [];

    for (let i = 1; i <= this.computerViewData.pagination.pages; i ++){
      pages.push(i);
    }

    return pages;
  }

}
