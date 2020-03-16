import {IApplication} from "./application.interface";

export interface IComputerInfo {
  name: string;
  date: Date;
  apps: IApplication[];
}
