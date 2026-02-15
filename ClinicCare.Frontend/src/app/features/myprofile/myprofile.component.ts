import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Location } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MaterialModule } from '../../shared/ui/material.module';
import { PatientUpdateDto } from '../../shared/models/patient.model';
import { EmployeeUpdateDto } from '../../shared/models/employee.model';
import { AuthService } from '../../services/auth.service';
import { PatientService } from '../../services/patient.service';
import { EmployeeService } from '../../services/employee.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-my-profile',
  imports: [ReactiveFormsModule, MaterialModule],
  templateUrl: './myprofile.component.html',
})
export class MyProfileComponent implements OnInit {
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private patientService = inject(PatientService);
  private employeeService = inject(EmployeeService);
  private location = inject(Location);
  private snackBar = inject(MatSnackBar);

  role = signal<string>(this.authService.role || 'Patient');
  userId = signal<string>(this.authService.userId || '');

  loading = signal(false);
  saving = signal(false);
  profileLoaded = signal(false);

  form = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: [{ value: '', disabled: true }],
    phone: ['', [Validators.required, Validators.pattern(/^\d{10,15}$/)]],
    password: ['', [Validators.minLength(8)]],

    emergencyContact: [''],
    bloodGroup: [''],
    allergies: [''],
    height: [null as number | null],
    bodyWeight: [null as number | null],
    address: [''],

    fee: [null as number | null],
    specializationId: [''],
  });

  isPatient = computed(() => this.role() === 'Patient');
  isDoctor = computed(() => this.role() === 'Doctor');
  isAdmin = computed(() => this.role() === 'Admin');
  isEmployee = computed(() => this.isDoctor() || this.isAdmin());

  ngOnInit() {
    this.loadProfile();
  }

  loadProfile() {
    this.loading.set(true);
    const userId = this.userId();

    if (!userId) {
      this.showError('User ID not found');
      this.loading.set(false);
      return;
    }

    if (this.isPatient()) {
      this.patientService.getById(userId).subscribe({
        next: (patient) => {
          if (patient) {
            this.form.patchValue({
              firstName: patient.firstName,
              lastName: patient.lastName,
              email: patient.email,
              phone: patient.phone,
            });
            this.profileLoaded.set(true);
          }
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Failed to load patient profile:', err);
          this.showError('Failed to load profile');
          this.loading.set(false);
        },
      });
    } else if (this.isEmployee()) {
      this.employeeService.getById(userId).subscribe({
        next: (employee) => {
          if (employee) {
            this.form.patchValue({
              firstName: employee.firstName,
              lastName: employee.lastName,
              email: employee.email,
              phone: employee.phone,
              fee: employee.fee || null,
              specializationId: employee.specializationId || '',
            });
            this.profileLoaded.set(true);
          }
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Failed to load employee profile:', err);
          this.showError('Failed to load profile');
          this.loading.set(false);
        },
      });
    }
  }

  save() {
    if (this.form.invalid) {
      this.showError('Please fill in all required fields correctly');
      return;
    }

    this.saving.set(true);
    const userId = this.userId();

    if (this.isPatient()) {
      const payload: PatientUpdateDto = this.buildPatientPayload();

      this.patientService.update(userId, payload).subscribe({
        next: () => {
          this.showSuccess('Profile updated successfully');
          this.saving.set(false);
          this.form.patchValue({ password: '' });
        },
        error: (err) => {
          console.error('Failed to update patient profile:', err);
          this.showError('Failed to update profile');
          this.saving.set(false);
        },
      });
    } else if (this.isEmployee()) {
      const payload: EmployeeUpdateDto = this.buildEmployeePayload();

      this.employeeService.update(userId, payload).subscribe({
        next: () => {
          this.showSuccess('Profile updated successfully');
          this.saving.set(false);
          this.form.patchValue({ password: '' });
        },
        error: (err) => {
          console.error('Failed to update employee profile:', err);
          this.showError('Failed to update profile');
          this.saving.set(false);
        },
      });
    }
  }

  private buildPatientPayload(): PatientUpdateDto {
    const raw = this.form.getRawValue();

    const payload: PatientUpdateDto = {
      firstName: raw.firstName || undefined,
      lastName: raw.lastName || undefined,
      phone: raw.phone || undefined,
      address: raw.address || undefined,
      emergencyContact: raw.emergencyContact || undefined,
      bloodGroup: raw.bloodGroup || undefined,
      allergies: raw.allergies || undefined,
      bodyWeight: raw.bodyWeight || undefined,
      height: raw.height || undefined,
    };

    if (raw.password && raw.password.trim() !== '') {
      payload.password = raw.password;
    }

    return payload;
  }

  private buildEmployeePayload(): EmployeeUpdateDto {
    const raw = this.form.getRawValue();

    const payload: EmployeeUpdateDto = {
      firstName: raw.firstName || undefined,
      lastName: raw.lastName || undefined,
      phone: raw.phone || undefined,
    };

    if (this.isDoctor()) {
      if (raw.fee !== null) {
        payload.fee = raw.fee;
      }
      if (raw.specializationId) {
        payload.specializationId = raw.specializationId;
      }
    }

    if (raw.password && raw.password.trim() !== '') {
      payload.password = raw.password;
    }

    return payload;
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

  goBack() {
    this.location.back();
  }

  reset() {
    this.loadProfile();
  }
}