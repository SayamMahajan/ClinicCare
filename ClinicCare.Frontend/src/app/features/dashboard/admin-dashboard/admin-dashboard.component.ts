import { Component } from '@angular/core';
import {
  AnalyticsComponent,
  AnalyticsStat,
} from '../../../shared/components/analytics/analytics.component';

@Component({
  selector: 'app-admin-dashboard',
  imports: [AnalyticsComponent],
  templateUrl: './admin-dashboard.component.html',
})
export class AdminDashboardComponent {
  stats: AnalyticsStat[] = [
    {
      label: "Today's Appointments",
      value: '--',
    },
    {
      label: 'Patients Today',
      value: '--',
    },
    {
      label: 'Active Doctors',
      value: '--',
    },
  ];
}
