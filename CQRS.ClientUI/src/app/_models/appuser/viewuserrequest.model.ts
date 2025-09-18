export interface ViewUserRequest {
  id: number;
  firstName: string;
  lastName: string;
  email: string;
  username: string;
  password: string;
  confirmPassword: string;
  token: string;
  isActive: string;
  role: string;
}
