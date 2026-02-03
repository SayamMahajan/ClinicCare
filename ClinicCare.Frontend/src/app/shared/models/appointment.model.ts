export type TimeSlot = 
  |'Morning' 
  | 'EarlyNoon' 
  | 'LateNoon' 
  | 'Evening' 
  | 'Night';

export type AppointmentStatus = 
  |'Requested'
  | 'Approved'
  | 'Cancelled'
  | 'Completed';

export type PastTimeRange =
  | 'All'
  | 'Past Day'
  | 'Past Week'
  | 'Past Month'

export type FutureTimeRange =
  | 'All'
  | 'Next Day'
  | 'Next Week'
  | 'Next Month'

export type UserRole = 'Doctor' | 'Patient';

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
  patientId: string
  doctorId: string
  date: string
  timeSlot: TimeSlot
}

export interface AppointmentResponseDto {
  id: string;
  status: AppointmentStatus;
  date: string;
  timeSlot: TimeSlot;
  patient: PatientMiniDto;
  doctor: DoctorMiniDto;
}
export interface DoctorDto {
  id: string;
  firstName: string;
  lastName: string;
  specialization: string;

  specializationId?: string;
  fee?: number;
  phone?: string;
  firstPracticeDate?: string;
}
