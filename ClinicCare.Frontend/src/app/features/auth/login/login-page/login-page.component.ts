import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoginFormComponent } from '../login-form/login-form.component';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { AuthUserType, LoginFormValue } from '../../../../shared/models/auth.models';
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
  
  userType = signal<AuthUserType>('patient');

  private typeParam = toSignal(
    this.route.paramMap,
    { initialValue: this.route.snapshot.paramMap }
  );
  
  constructor() {
    effect(() => {
      const type = this.typeParam().get('type');

      if (type === 'patient' || type === 'employee') {
        this.userType.set(type);
      } else {
        this.router.navigate(['/']);
      }
    });
  }
  
  onSubmit(data: LoginFormValue) {
    const request$ =
      this.userType() === 'patient'
        ? this.authService.loginPatient(data)
        : this.authService.loginEmployee(data);

    request$.subscribe({
      next: res => {
        localStorage.setItem('token', res.token);

        if (this.userType() === 'patient') {
          this.router.navigate(['/patient/dashboard']);
          return;
        }

        const role = 'Doctor'// this.authService.getRoleFromToken();

        if (role === 'Doctor') {
          this.router.navigate(['/doctor/dashboard']);
        } else if (role === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        } else {
          console.error('Unknown role', role);
          this.router.navigate(['/']);
        }
      },
      error: err => {
        console.error('Login failed', err);
        alert('Invalid email or password');
      }
    });
  }
}
