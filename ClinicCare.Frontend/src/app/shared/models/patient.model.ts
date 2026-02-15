export type Gender = 'Male' | 'Female' | 'Others';

export interface PatientLoginDto {
  email: string;
  password: string;
}

export interface PatientLoginResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
}

export interface PatientRegisterDto {
  firstName: string;
  lastName: string;
  dob: string;
  gender: Gender;
  email: string;
  phone: string;
  password: string;
}

export interface PatientResponseDto {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
}

export interface PatientUpdateDto {
  firstName?: string;
  lastName?: string;
  password?: string;
  phone?: string;
  emergencyContact?: string;
  bloodGroup?: string;
  allergies?: string;
  bodyWeight?: number;
  height?: number;
  address?: string;
}

export interface PatientMiniDto {
  id: string;
  firstName: string;
  lastName: string;
}