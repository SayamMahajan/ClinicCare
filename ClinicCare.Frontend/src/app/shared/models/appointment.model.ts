export type TimeSlot = 'Morning' | 'Earlynoon' | 'Latenoon' | 'Evening' | 'Night';
export type AppointmentStatus = 'Requested' | 'Approved' | 'Cancelled' | 'Completed';

export type PastTimeRange = 'All' | 'Past Day' | 'Past Week' | 'Past Month';
export type FutureTimeRange = 'All' | 'Next Day' | 'Next Week' | 'Next Month';

export interface PatientMiniDto {
  id: string;
  firstName: string;
  lastName: string;
}

export interface DoctorMiniDto {
  id: string;
  firstName: string;
  lastName: string;
}

export interface AppointmentCreateDto {
  patientId: string;
  doctorId: string;
  date: string;
  paymentId: string;
  timeSlot: TimeSlot;
}

export interface AppointmentUpdateDto {
  status?: AppointmentStatus;
  date?: string;
  timeSlot?: TimeSlot;
}

export interface AppointmentResponseDto {
  id: string;
  status: AppointmentStatus;
  prescriptionId?: string;
  date: string;
  timeSlot: TimeSlot;
  createdAt: string;
  patient: PatientMiniDto;
  doctor: DoctorMiniDto;
}