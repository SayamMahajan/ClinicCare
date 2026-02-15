import { PatientMiniDto } from './patient.model';
import { DoctorMiniDto } from './employee.model';

export type PaymentType = 'Paid' | 'Refund';

export interface PaymentCreateDto {
  amount: number;
  patientId: string;
  doctorId: string;
}

export interface PaymentResponseDto {
  id: string;
  transactionId: string;
  amount: number;
  patient: PatientMiniDto;
  doctor: DoctorMiniDto;
  type: PaymentType;
  createdAt: string; 
}