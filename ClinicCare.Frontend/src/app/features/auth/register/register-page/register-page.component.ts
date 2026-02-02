import { Component, effect, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RegisterFormComponent } from '../register-form/register-form.component';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { AuthUserType, EmployeeRegisterForm, PatientRegisterForm, RegisterFormValue } from '../../../../shared/models/auth.models';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { AuthService } from "../../../../shared/services/auth.service";


@Component({
  selector: 'app-register-page',
  imports: [CommonModule, RegisterFormComponent, MaterialModule],
  templateUrl: './register-page.component.html'
})
export class RegisterPageComponent {
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
  
  onRegister(data: any) {
    const request$ =
      this.userType() === 'patient'
        ? this.authService.registerPatient(data as PatientRegisterForm)
        : this.authService.registerEmployee(data as EmployeeRegisterForm);

    request$.subscribe({
      next: () => {
        this.router.navigate(['/auth/login', this.userType()]);
      },
      error: err => {
        console.error('Registration failed', err);
        alert(err.error?.message ?? 'Registration failed');
      }
    });
  }

}
