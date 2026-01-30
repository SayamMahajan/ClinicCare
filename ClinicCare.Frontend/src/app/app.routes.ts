import { Routes } from '@angular/router';
import { DoctorDashboardComponent } from './features/dashboard/doctor/doctor-dashboard.component';
import { PatientDashboardComponent } from './features/dashboard/patient/patient-dashboard.component';

export const routes: Routes = [
  { path: 'login', loadComponent: () =>import('./features/auth/login/login.component').then(m => m.LoginComponent) },
  { path: 'patient/register', loadComponent: () => import('./features/auth/patient-register/patient-register.component').then(m => m.PatientRegisterComponent) },
  { path: 'employee/register', loadComponent: () => import('./features/auth/employee-register/employee-register.component').then(m => m.EmployeeRegisterComponent) },
  { path: 'employee/doctor-dashboard', component: DoctorDashboardComponent },
  { path: 'patient/dashboard', component: PatientDashboardComponent },
  { path: 'appointment/:id', loadComponent: () => import('./features/dashboard/doctor/doctor-dashboard.component').then(m => m.DoctorDashboardComponent) },
];
