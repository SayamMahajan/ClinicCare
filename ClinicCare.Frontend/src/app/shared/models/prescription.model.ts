import { PatientMiniDto } from './patient.model';
import { DoctorMiniDto } from './employee.model';

export interface MedicationDto {
  medicine: string;
  dosage: number; 
  frequency: string;
  days: number; 
  instructions?: string;
}

export interface PrescriptionCreateDto {
  patientId: string;
  doctorId: string;
  description: MedicationDto[];
  appointmentId: string;
}

export interface PrescriptionResponseDto {
  id: string;
  patient: PatientMiniDto;
  doctor: DoctorMiniDto;
  description: MedicationDto[];
  createdAt: string; 
}

export interface PrescriptionDialogData {
  patientId: string;
  doctorId: string;
  patientName: string;
  doctorName: string;
  appointmentId: string;
}

export function getMedicationsString(prescription: PrescriptionResponseDto): string {
  return prescription.description.map((med) => med.medicine).join(', ');
}

export function getMedicationsSummary(prescription: PrescriptionResponseDto): string {
  const meds = prescription.description.map((med) => med.medicine);
  return meds.length > 2
    ? `${meds.slice(0, 2).join(', ')}... (+${meds.length - 2} more)`
    : meds.join(', ');
}