import { Component } from '@angular/core';
import { AnalyticsComponent, AnalyticsStat } from '../../../shared/components/analytics/analytics.component';

@Component({
  selector: 'app-patient-dashboard',
  imports: [AnalyticsComponent],  
  templateUrl: './patient-dashboard.component.html'
})
export class PatientDashboardComponent {
  stats: AnalyticsStat[] = [
    {
      label: 'Upcoming Appointments',
      value: '--',
    },
    {
      label: 'Completed Visits',
      value: '--',
    },
    {
      label: 'Pending Payments',
      value: '--',
    }
  ];
}
