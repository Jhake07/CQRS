export interface IjobOrder {
  id: number;
  joNo: string;
  batchSerial_ContractNo: string;
  stats: string;
  orderType: string;
  orderQty: number;
  startTime?: Date;
  endTime?: Date;
  line: string;
  processOrder: number;
  isNo: string;
  remainingQty: number;
}
