export interface Joborder {
  id: number;
  joNo: string;
  contractNo: string;
  stats?: string;
  orderType: string;
  orderQty: number;
  startTime?: Date;
  endTime?: Date;
  line1Qty?: number;
  processOrderQtyLine1?: number;
  line2Qty?: number;
  processOrderQtyLine2?: number;
  iSNo: string;
  leftQty: number;
}
