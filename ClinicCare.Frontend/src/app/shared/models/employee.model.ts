export type EmployeeDialogAction = 'update' | 'delete';

export interface Employee {
  id: string;
  firstName: string;
  lastName: string;
  role: 'Admin' | 'Doctor';
  email: string;
  dateOfJoining: string;
}

export interface DoctorResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  role: 'Admin' | 'Doctor';
  dateOfJoining: string;

  specializationId?: string;
  fee?: number;
  phone?: string;
}
