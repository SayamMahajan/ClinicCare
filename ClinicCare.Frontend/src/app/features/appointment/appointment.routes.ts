import { Routes } from '@angular/router';
import { DashboardLayoutComponent } from '../../shared/components/dashboard-layout/dashboard-layout.component';
import { patientGuard } from '../../guards/patient.guard';

export const APPOINTMENTS_ROUTES: Routes = [
  {
    path: 'requests',
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./requests/requests.component')
            .then(m => m.RequestsComponent) 
      }
    ]  
  },
  {
    path: 'scheduled',
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./scheduled/scheduled.component')
            .then(m => m.ScheduledComponent)
      }
    ]
  },
  {
    path: 'history',
    component: DashboardLayoutComponent,
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./history/history.component')
            .then(m => m.HistoryComponent)
      }
    ]
  },
  {
    path: 'book',
    component: DashboardLayoutComponent,
    canActivateChild: [patientGuard],
    children: [
      {
        path: '',
        loadComponent: () =>
          import('./book-appointment/book-appointment.component')
            .then(m => m.BookAppointmentComponent)
      }
    ]
  }
];
