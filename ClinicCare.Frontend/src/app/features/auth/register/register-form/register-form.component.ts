import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from '../../../../shared/ui/material.module';
import {
  EmployeeRole,
  Gender,
  PatientRegisterForm,
  EmployeeRegisterForm
} from '../../../../shared/models/auth.models';

@Component({
  selector: 'app-register-form',
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './register-form.component.html'
})
export class RegisterFormComponent {
  @Input({ required: true }) userType!: 'patient' | 'employee';
  @Output() submitted = new EventEmitter<PatientRegisterForm | EmployeeRegisterForm>();

  private fb = inject(FormBuilder);

  employeeRole = signal<EmployeeRole>('Admin');

  patientForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    dob: [null as any, Validators.required],
    gender: ['Male' as Gender, Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    password: ['', Validators.required]
  });

  employeeForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    dateOfJoining: [null as any, Validators.required],
    role: ['Admin' as EmployeeRole],
    doctorDetails: this.fb.group({
      specializationId: ['' , Validators.required],
      dob: [null as Date | null, Validators.required],
      firstPracticeDate: [null as Date | null, Validators.required],
      fee: ['' , Validators.required],
      phone: ['', Validators.required],
    })
  });

  private mapEmployeeDto(form: any) {
    if (form.role === 'Doctor') {
      return {
        firstName: form.firstName,
        lastName: form.lastName,
        email: form.email,
        password: form.password,
        dateOfJoining: form.dateOfJoining,
        role: form.role,
        doctorDetails: form.doctorDetails
      };
    }

    return {
      firstName: form.firstName,
      lastName: form.lastName,
      email: form.email,
      password: form.password,
      dateOfJoining: form.dateOfJoining,
      role: form.role
    };
  }

  submit() {
    if (this.userType === 'patient') {
      if (this.patientForm.invalid) return;
      this.submitted.emit(this.patientForm.getRawValue());
    }

    if (this.userType === 'employee') {
      if (this.employeeForm.invalid) return;

      const raw = this.employeeForm.getRawValue();
      const dto = this.mapEmployeeDto(raw);

      this.submitted.emit(dto);
    }
  }
}
