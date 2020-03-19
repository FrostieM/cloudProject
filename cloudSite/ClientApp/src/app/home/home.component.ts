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

  constructor(private computerService: ComputerService) {
  }


}
