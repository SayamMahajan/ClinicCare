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
  patient: {
    id: string;
    firstName: string;
    lastName: string;
  };
  doctor: {
    id: string;
    firstName: string;
    lastName: string;
  };
  description: MedicationDto[];
}

export interface PrescriptionDialogData {
  patientId: string;
  doctorId: string;
  patientName: string;
  doctorName: string;
  appointmentId: string;
}