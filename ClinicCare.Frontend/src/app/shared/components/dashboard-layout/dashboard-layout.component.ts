import { Component, computed, inject, Input, signal } from '@angular/core';
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
  private authService = inject(AuthService);
  private router = inject(Router);
  
  onLogout() {
    this.authService.logout();
    this.router.navigate(['/auth']);
  }

  onProfile() {
    this.router.navigate(['/my-profile']);
  }
}
