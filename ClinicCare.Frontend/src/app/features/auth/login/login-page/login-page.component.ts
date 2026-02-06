import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from '../login-form/login-form.component';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { AuthUser, LoginFormValue } from '../../../../shared/models/auth.model';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from "../../../../shared/services/auth.service";

@Component({
  selector: 'app-login-page',
  imports: [CommonModule, MaterialModule, LoginFormComponent],
  templateUrl: './login-page.component.html'
})
export class LoginPageComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  
  userType = signal<AuthUser>('patient');

  private typeParam = toSignal(
    this.route.paramMap,
    { initialValue: this.route.snapshot.paramMap }
  );
  
  constructor() {
    effect(() => {
      const type = this.typeParam().get('type');

      if (type === 'patient' || type === 'employee') {
        if(type === 'employee')
          this.userType.set('doctor');
        else
          this.userType.set('patient');
      } else {
        this.router.navigate(['/']);
      }
    });
  }
  
  onLogin(data: LoginFormValue) {
    const request$ =
      this.userType() === 'patient'
        ? this.authService.loginPatient(data)
        : this.authService.loginEmployee(data);

    request$.subscribe({
      next: res => {
        localStorage.setItem('token', res.token);

        if (this.userType() === 'patient') {
          this.router.navigate(['/appointments/scheduled']);
          return;
        }

        const role = signal(this.authService.role);

        if (role() === 'Doctor') {
          this.router.navigate(['/appointments/scheduled']);
        } else if (role() === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        } else {
          this.router.navigate(['/']);
        }
      },
      error: err => {
        alert('Invalid email or password');
      }
    });
  }
}
