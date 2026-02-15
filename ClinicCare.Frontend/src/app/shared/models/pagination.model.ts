import { AppointmentStatus } from './appointment.model';
import { PaymentType } from './payment.model';
import { Gender, EmployeeRole } from './employee.model';

export interface PaginationParams {
  pageNumber: number;
  pageSize: number;
}

export interface PaginatedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface AppointmentSearchParams extends PaginationParams {
  searchTerm?: string;
  status?: AppointmentStatus;
  prescriptionId?: string;
  startDate?: string;
  endDate?: string; 
}

export interface EmployeeSearchParams extends PaginationParams {
  searchTerm?: string;
  role: EmployeeRole;
  gender?: Gender;
  specializationId?: string;
}

export interface PaymentSearchParams extends PaginationParams {
  searchTerm?: string;
  patientId?: string;
  doctorId?: string;
  type?: PaymentType;
  startDate?: string;
  endDate?: string;  
}

export interface PrescriptionSearchParams extends PaginationParams {
  searchTerm?: string;
  patientId?: string;
  doctorId?: string;
  startDate?: string;
  endDate?: string; 
}
