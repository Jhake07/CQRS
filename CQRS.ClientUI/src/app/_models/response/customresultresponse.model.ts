// --- Custom Result Response Model for API Responses ---
export interface CustomResultResponse {
  isSuccess: boolean;
  message: string;
  id?: string;
  validationErrors?: { [key: string]: string[] };
}
// --- Optional Generic Version for Data Payloads(Typed Queries) ---
export interface CustomResultResponseWithData<T> extends CustomResultResponse {
  data?: T;
}
