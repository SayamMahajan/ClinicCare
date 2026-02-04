export type Role = 'Admin' | 'Doctor' | 'Patient';

export interface MiniUser  {
  id: string;
  firstName: string;
  lastName: string;
}

export interface Payment {
  id: string;
  amount: number;
  patient: MiniUser ;
  doctor: MiniUser ;
  createdAt: string;
}
