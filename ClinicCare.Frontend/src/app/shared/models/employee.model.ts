export type EmployeeRole = 'Admin' | 'Doctor';
export type Gender = 'Male' | 'Female' | 'Others';
export type EmployeeDialogAction = 'update' | 'delete' | 'cancel';

export interface EmployeeLoginDto {
  email: string;
  password: string;
}

export interface EmployeeLoginResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: EmployeeRole;
  token: string;
}

export interface DoctorRegisterDetailsDto {
  specializationId: string;
  fee: number;
  firstPracticeDate: string;
}

export interface EmployeeRegisterDto {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  role: EmployeeRole;
  dob: string;
  gender: Gender;
  phone: string;
  doctorDetails?: DoctorRegisterDetailsDto;
}

export interface EmployeeResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: EmployeeRole;
  createdAt: string; 
  gender: Gender;
  phone: string;
  dob: string; 
  specializationId?: string;
  fee?: number;
  firstPracticeDate?: string; 
}

export interface EmployeeUpdateDto {
  firstName?: string;
  lastName?: string;
  password?: string;
  fee?: number;
  specializationId?: string;
  phone?: string;
}

export interface AdminDashboardResponse {
  appointmentsToday: number;
  appointmentsThisMonth: number;
  totalDoctors: number;
  newPatientsToday: number;
  newPatientsThisMonth: number;
}

export interface DoctorMiniDto {
  id: string;
  firstName: string;
  lastName: string;
}