export interface PatientDetails {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  password: string;
  address?: string;
  emergencyContact?: string;
  bloodGroup?: string;
  allergies?: string;
  bodyWeight?: number;
  height?: number;
}

export interface PatientUpdate {
  firstName?: string;
  lastName?: string;
  phone?: string;
  address?: string;
  password?: string;
  emergencyContact?: string;
  bloodGroup?: string;
  allergies?: string;
  bodyWeight?: number;
  height?: number;
}
