import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../../shared/ui/material.module';
import { AppToolbarComponent } from '../../../shared/components/app-toolbar/app-toolbar.component';
import { AppSidenavComponent } from '../../../shared/components/app-sidenav/app-sidenav.component';

@Component({
  selector: 'app-dashboard-layout',
  imports: [CommonModule, RouterOutlet, MaterialModule, AppToolbarComponent, AppSidenavComponent],
  templateUrl: './dashboard-layout.component.html'
})
export class DashboardLayoutComponent {
  role = computed(() => 'Admin' as 'Admin' | 'Doctor' | 'Patient');
}
