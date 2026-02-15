import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  ValidatorFn,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { Router } from '@angular/router';
import { MaterialModule } from '../../../shared/ui/material.module';
import { EmployeeRegisterDto, EmployeeRole, Gender } from '../../../shared/models/employee.model';
import { SpecializationResponseDto } from '../../../shared/models/specialization.model';
import { AuthService } from '../../../services/auth.service';
import { SpecializationService } from '../../../services/specialization.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-add-employees',
  imports: [ReactiveFormsModule, MaterialModule],
  templateUrl: './add-employees.component.html',
})
export class AddEmployeesComponent implements OnInit {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private specializationService = inject(SpecializationService);
  private router = inject(Router);
  private location = inject(Location);
  private snackBar = inject(MatSnackBar);

  specializations = signal<SpecializationResponseDto[]>([]);
  loadingSpecializations = signal(false);
  submitting = signal(false);

  minDob = new Date(new Date().getFullYear() - 70, 0, 1);
  maxDob = new Date(new Date().getFullYear() - 18, 11, 31); // Must be at least 18
  minPracticeDate = new Date(1970, 0, 1);
  maxPracticeDate = new Date();

  form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100)]],
    password: ['', [Validators.required, Validators.minLength(8), this.passwordValidator()]],
    role: ['Doctor' as EmployeeRole, Validators.required],
    dob: [null as Date | null, Validators.required],
    gender: ['Male' as Gender, Validators.required],
    phone: ['', [Validators.required, this.phoneValidator()]],

    specializationId: [''],
    fee: [null as number | null],
    firstPracticeDate: [null as Date | null],
  });

  ngOnInit() {
    this.loadSpecializations();

    this.form.controls.role.valueChanges.subscribe((role) => {
      this.updateDoctorFieldsValidation(role);
    });

    this.updateDoctorFieldsValidation(this.form.controls.role.value);
  }

  loadSpecializations() {
    this.loadingSpecializations.set(true);

    this.specializationService.getAll({ pageNumber: 1, pageSize: 100 }).subscribe({
      next: (result) => {
        this.specializations.set(result.items);
        this.loadingSpecializations.set(false);
      },
      error: (err) => {
        console.error('Failed to load specializations:', err);
        this.specializations.set([]);
        this.loadingSpecializations.set(false);
      },
    });
  }

  updateDoctorFieldsValidation(role: EmployeeRole) {
    if (role === 'Doctor') {
      this.form.controls.specializationId.setValidators([Validators.required]);
      this.form.controls.fee.setValidators([Validators.required, Validators.min(0)]);
      this.form.controls.firstPracticeDate.setValidators([Validators.required]);
    } else {
      this.form.controls.specializationId.clearValidators();
      this.form.controls.fee.clearValidators();
      this.form.controls.firstPracticeDate.clearValidators();
    }

    this.form.controls.specializationId.updateValueAndValidity();
    this.form.controls.fee.updateValueAndValidity();
    this.form.controls.firstPracticeDate.updateValueAndValidity();
  }

  isDoctor = signal(true);

  onRoleChange(role: EmployeeRole) {
    this.isDoctor.set(role === 'Doctor');
  }

  phoneValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;
      const phoneRegex = /^\d{10,15}$/;
      return phoneRegex.test(value) ? null : { invalidPhone: true };
    };
  }

  passwordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;
      if (!value) return null;
      const strongPasswordRegex =
        /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
      return strongPasswordRegex.test(value) ? null : { weakPassword: true };
    };
  }

  formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.showError('Please fill in all required fields correctly');
      return;
    }

    const raw = this.form.getRawValue();
    this.submitting.set(true);

    const payload: EmployeeRegisterDto = {
      firstName: raw.firstName,
      lastName: raw.lastName,
      email: raw.email,
      password: raw.password,
      role: raw.role,
      dob: this.formatDate(raw.dob!),
      gender: raw.gender,
      phone: raw.phone,
    };

    if (raw.role === 'Doctor') {
      payload.doctorDetails = {
        specializationId: raw.specializationId,
        fee: raw.fee!,
        firstPracticeDate: this.formatDate(raw.firstPracticeDate!),
      };
    }

    this.authService.registerEmployee(payload).subscribe({
      next: () => {
        this.submitting.set(false);
        this.showSuccess('Employee registered successfully!');

        setTimeout(() => {
          this.router.navigate(['/employees']);
        }, 1000);
      },
      error: (err) => {
        this.submitting.set(false);
        console.error('Employee registration failed:', err);

        const errorMessage =
          err.error?.message || 'Registration failed. Please try again.';
        this.showError(errorMessage);
      },
    });
  }

  goBack() {
    this.location.back();
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