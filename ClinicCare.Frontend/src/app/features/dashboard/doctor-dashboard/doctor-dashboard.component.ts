import { Component } from '@angular/core';
import { AnalyticsComponent, AnalyticsStat } from '../../../shared/components/analytics/analytics.component';

@Component({
  selector: 'app-doctor-dashboard',
  imports: [AnalyticsComponent],
  templateUrl: './doctor-dashboard.component.html',
})
export class DoctorDashboardComponent {
  stats: AnalyticsStat[] = [
    {
      label: "Today's Appointments",
      value: '--',
    },
    {
      label: 'Pending Requests',
      value: '--',
    },
    {
      label: 'Completed Today',
      value: '--',
    },
    {
      label: 'Total Patients',
      value: '--',
    }
  ];
}
