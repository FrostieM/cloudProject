import {Component, OnInit} from '@angular/core';
import {ComputerService} from "../shared/services/computer.service";
import {IComputerViewData} from "../shared/interfaces/computerViewData.interface";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-program-list',
  templateUrl: './programList.component.html',
  styleUrls: ['programList.component.css']
})
export class ProgramListComponent implements OnInit{

  private _computerViewData: IComputerViewData;

  constructor(private computerService: ComputerService, private route: ActivatedRoute) {
  }

  ngOnInit(): void {
    let compName = this.route.snapshot.paramMap.get('compName');
    this.getComputerInfo(compName, 1);
  }

  public getComputerInfo(computerName: string, page: number){
    this.computerService.getComputerInfo(computerName, page).subscribe(
      request => this._computerViewData = request
    );
  }

  public getNumberRange(){
    let arr = [];

    let start = this._computerViewData.pagination.currentPage - 3;
    let end = this._computerViewData.pagination.currentPage + 2;

    for (let i = start; i <= end; i++)
      if (i > 1 && i < this._computerViewData.pagination.pages)
        arr.push(i);

    return arr;
  }

}
