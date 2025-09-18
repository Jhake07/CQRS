export interface BatchSerial {
  id: number;
  contractNo: string;
  customer: string;
  address: string;
  docNo: string;
  batchQty: number;
  orderQty: number;
  deliverQty: number;
  status: string;
  serialPrefix: string;
  startSNo: string;
  endSNo: string;
  item_ModelCode: string;
}
