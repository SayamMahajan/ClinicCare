export interface PatientProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

export interface PatientUpdate {
  firstName: string;
  lastName: string;
  phone: string;
  address: string;
  password: string;
  emergencyContact?: string;
  bloodGroup?: string;
  allergies?: string;
  bodyWeight?: number;
  height?: number;
}
