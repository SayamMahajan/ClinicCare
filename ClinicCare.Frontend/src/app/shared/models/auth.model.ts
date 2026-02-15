export type AuthUser = 'employee' | 'patient';
export type UserRole = 'Admin' | 'Doctor' | 'Patient';

export interface LoginFormValue {
  email: string;
  password: string;
}

export interface AuthResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
  role?: string; 
}