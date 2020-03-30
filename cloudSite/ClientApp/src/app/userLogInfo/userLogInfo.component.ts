import {Component, OnInit} from '@angular/core';
import {UserLogInfoService} from "../shared/services/user-log-info.service";
import {UserLogInfo} from "../shared/interfaces/userLogInfo.interface";

@Component({
  selector: 'app-usersLogInfo',
  templateUrl: './userLogInfo.component.html',
  styleUrls: ['userLogInfo.component.css']
})
export class UserLogInfoComponent implements OnInit{
  public usersLogInfo: UserLogInfo[];

  constructor(private _userLogInfoService: UserLogInfoService) {
  }

  ngOnInit(): void {
    this._userLogInfoService.getUsersLogInfo().subscribe(request => this.usersLogInfo = request);
  }

}
