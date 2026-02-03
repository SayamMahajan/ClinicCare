import { Component, computed, inject, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../ui/material.module';
import { AppToolbarComponent } from '../app-toolbar/app-toolbar.component';
import { AppSidenavComponent } from '../app-sidenav/app-sidenav.component';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-dashboard-layout',
  imports: [CommonModule, RouterOutlet, MaterialModule, AppToolbarComponent, AppSidenavComponent],
  templateUrl: './dashboard-layout.component.html'
})
export class DashboardLayoutComponent {
  private auth = inject(AuthService);
  private router = inject(Router);

  // role = computed(() => this.employeeStore.role() ?? 'Patient');
  
  @Input() role: 'Admin' | 'Doctor' | 'Patient' = 'Admin';

  onLogout() {
    this.auth.logout();
    this.router.navigate(['/auth']);
  }

  onProfile() {
    this.router.navigate(['/profile']);
  }
}
