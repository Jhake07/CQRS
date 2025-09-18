export interface AuthResponse {
  id: string;
  userName: string;
  email: string;
  token: string;
  isSuccess: boolean;
  message: string;
  expiresAt?: number; //this is only for the front end session
}
