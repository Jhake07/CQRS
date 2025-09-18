export interface CustomResultResponse {
  isSuccess: boolean;
  message: string;
  id?: string;
  validationErrors?: { [key: string]: string[] }; // Dictionary-like error list
}
