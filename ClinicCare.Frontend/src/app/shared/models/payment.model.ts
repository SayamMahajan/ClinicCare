export type Role = 'Admin' | 'Doctor' | 'Patient';

export interface PersonMiniDto {
  id: string;
  firstName: string;
  lastName: string;
}

export interface Payment {
  id: string;
  amount: number;
  patient: PersonMiniDto;
  doctor: PersonMiniDto;
  createdAt: string;
}
