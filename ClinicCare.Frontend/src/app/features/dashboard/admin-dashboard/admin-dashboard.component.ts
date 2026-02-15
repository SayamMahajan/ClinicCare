import { Component, inject, OnInit, signal } from '@angular/core';
import {
  AnalyticsComponent,
  AnalyticsStat,
} from '../../../shared/components/analytics/analytics.component';
import { EmployeeService } from '../../../services/employee.service';
import { MaterialModule } from '../../../shared/ui/material.module';

@Component({
  selector: 'app-admin-dashboard',
  imports: [AnalyticsComponent, MaterialModule],
  templateUrl: './admin-dashboard.component.html',
})
export class AdminDashboardComponent implements OnInit {
  private employeeService = inject(EmployeeService);

  stats = signal<AnalyticsStat[]>([
    {
      label: "Today's Appointments",
      value: '0',
    },
    {
      label: 'Monthly Appointments',
      value: '0',
    },
    {
      label: 'Active Doctors',
      value: '0',
    },
    {
      label: 'New Patients Today',
      value: '0',
    },
    {
      label: 'New Patients This Month',
      value: '0',
    },
  ]);

  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading.set(true);
    this.error.set(null);

    this.employeeService.getAdminDashboard().subscribe({
      next: (data) => {
        if (data) {
          this.stats.set([
            {
              label: "Today's Appointments",
              value: data.appointmentsToday.toString(),
            },
            {
              label: 'Monthly Appointments',
              value: data.appointmentsThisMonth.toString(),
            },
            {
              label: 'Active Doctors',
              value: data.totalDoctors.toString(),
            },
            {
              label: 'New Patients Today',
              value: data.newPatientsToday.toString(),
            },
            {
              label: 'New Patients This Month',
              value: data.newPatientsThisMonth.toString(),
            },
          ]);
        }
        this.loading.set(false);
      },
      error: (err) => {
        console.error('Failed to load dashboard data:', err);
        this.error.set('Failed to load dashboard data. Please try again.');
        this.loading.set(false);
      },
    });
  }

  retry() {
    this.loadDashboardData();
  }
}
