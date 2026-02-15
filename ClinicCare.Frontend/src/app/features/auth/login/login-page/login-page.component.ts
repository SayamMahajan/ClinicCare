import { Component, effect, inject, signal } from '@angular/core';
import { LoginFormComponent } from '../login-form/login-form.component';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { AuthUser, LoginFormValue } from '../../../../shared/models/auth.model';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-login-page',
  imports: [ MaterialModule, LoginFormComponent],
  templateUrl: './login-page.component.html',
})
export class LoginPageComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);

  userType = signal<AuthUser>('patient');
  loading = signal(false);

  private typeParam = toSignal(this.route.paramMap, {
    initialValue: this.route.snapshot.paramMap,
  });

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

  onLogin(data: LoginFormValue) {
    this.loading.set(true);

    const request$ =
      this.userType() === 'patient'
        ? this.authService.loginPatient(data)
        : this.authService.loginEmployee(data);

    request$.subscribe({
      next: (res) => {
        this.loading.set(false);
        this.showSuccess(`Welcome back, ${res.firstName}!`);

        if (this.userType() === 'patient') {
          this.router.navigate(['/appointments/scheduled']);
          return;
        }

        const role = this.authService.role;

        if (role === 'Doctor') {
          this.router.navigate(['/appointments/scheduled']);
        } else if (role === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        } else {
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        this.loading.set(false);
        console.error('Login failed:', err);

        const errorMessage =
          err.error?.message || 'Invalid email or password. Please try again.';
        this.showError(errorMessage);
      },
    });
  }

  private showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['success-snackbar'],
    });
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['error-snackbar'],
    });
  }
}