export type AuthUser = 'doctor' | 'patient';
export type EmployeeRole = 'Admin' | 'Doctor';
export type Gender = 'Male' | 'Female' | 'Other';

export interface LoginFormValue {
  email: string;
  password: string;
}

export interface PatientRegisterForm {
  firstName: string;
  lastName: string;
  dob: Date;
  gender: Gender;
  email: string;
  phone: string;
  password: string;
}

export interface DoctorDetailsForm {
  specializationId: string;
  fee: number;
  dob: Date;
  phone: string;
  firstPracticeDate: Date;
}

export interface DoctorRegisterForm {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  dateOfJoining: Date;
  role: EmployeeRole;
  doctorDetails: DoctorDetailsForm;
}

export interface AuthResponse {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
}