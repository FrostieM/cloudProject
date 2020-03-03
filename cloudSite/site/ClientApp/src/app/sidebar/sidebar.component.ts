import {Component, EventEmitter, Inject, OnInit, Output} from '@angular/core';
import {IComputer} from "../shared/interfaces/computer.interface";
import {ComputerService} from "../shared/services/computer.service";

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.css'],
})
export class SidebarComponent implements OnInit{
  public computers: IComputer[];
  @Output() messageToShowComputerInfo = new EventEmitter<string>();

  private currentComputer: IComputer;
  constructor(private computerService: ComputerService) {
  }

  ngOnInit(): void {
    this.computerService.getComputers().subscribe(request => {
      this.computers = request;
    });
  }

  public selectComputer(computer: IComputer){
    this.currentComputer = computer;
    this.messageToShowComputerInfo.emit(computer.name);
  }

}
