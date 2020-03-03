import {IComputerInfo} from "./computerInfo.interface";
import {IPagionation} from "./pagination.interface";

export interface IComputerViewData {
  computerInfo: IComputerInfo;
  pagination: IPagionation;
}
