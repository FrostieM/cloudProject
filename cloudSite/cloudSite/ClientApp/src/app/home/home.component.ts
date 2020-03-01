import { Component } from '@angular/core';
import {IComputer} from "../shared/interfaces/computer.interface";
import {ComputerService} from "../shared/services/computer.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['home.component.css']
})
export class HomeComponent {

  public programs: string[] = [];

  private _computer: string;
  public set Computer(computer: string){
    console.log(computer);
    this._computer = computer;
    this.computerService.getComputerInfo(this.Computer).subscribe(
      request => this.programs = request
    )
  }

  public get Computer(){
    return this._computer;
  }

  constructor(private computerService: ComputerService) {
  }


}
