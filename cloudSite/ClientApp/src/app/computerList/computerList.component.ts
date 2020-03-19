import {Component, OnInit} from '@angular/core';
import {ComputerService} from "../shared/services/computer.service";
import {IComputer} from "../shared/interfaces/computer.interface";
import {Router} from "@angular/router";

@Component({
  selector: 'app-computer-list',
  templateUrl: './computerList.component.html',
  styleUrls: ['computerList.component.css']
})
export class ComputerListComponent implements OnInit{

  public computers: IComputer[];

  constructor(private _computerService: ComputerService, private _router: Router) {
  }

  public ngOnInit(): void {
    this._computerService.getComputers().subscribe(request => {
      this.computers = request;
    });

    let input = document.getElementById('search');
    let timeout = null;
    let $this = this;

    input.addEventListener('keyup', function (_) {
      clearTimeout(timeout);
      timeout = setTimeout(function () {
        $this.getCompsByProgram((input as HTMLInputElement).value);
      }, 1000);
    });
  }

  public redirectToComputerInfo(compName: string){
    this._router.navigate(['/programList/' + compName]).then();
  }

  public getCompsByProgram(programName: string){
    this._computerService.getComputersByProgram(programName)
      .subscribe(
        request => this.computers = request,
        error => console.log(error));
  }

}
