import { Component, inject, signal } from '@angular/core';
import { RegisterFormComponent } from '../register-form/register-form.component';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { PatientRegisterDto } from '../../../../shared/models/patient.model';
import { Router } from '@angular/router';
import { AuthService } from '../../../../services/auth.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-register-page',
  imports: [RegisterFormComponent, MaterialModule],
  templateUrl: './register-page.component.html',
})
export class RegisterPageComponent {
  private router = inject(Router);
  private authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);

  loading = signal(false);

  imagePath = this.authService.bgImagePath;

  onRegister(data: PatientRegisterDto) {
    this.loading.set(true);

    this.authService.registerPatient(data).subscribe({
      next: () => {
        this.loading.set(false);
        this.showSuccess('Registration successful! Please login to continue.');

        setTimeout(() => {
          this.router.navigate(['/auth/login', 'patient']);
        }, 1500);
      },
      error: (err) => {
        this.loading.set(false);
        console.error('Registration failed', err);

        const errorMessage =
          err.error?.message || 'Registration failed. Please try again.';
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