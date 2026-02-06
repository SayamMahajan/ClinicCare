import { Component, EventEmitter, inject, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from '../../../../shared/ui/material.module';
import {
  DoctorRegisterForm,
  EmployeeRole,
  Gender,
  PatientRegisterForm,
} from '../../../../shared/models/auth.model';

@Component({
  selector: 'app-register-form',
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './register-form.component.html',
})
export class RegisterFormComponent {
  @Input({ required: true }) userType!: 'patient' | 'doctor';
  @Output() submitted = new EventEmitter<PatientRegisterForm | DoctorRegisterForm>();

  private fb = inject(FormBuilder);

  minDob = new Date(new Date().getFullYear() - 120, new Date().getMonth(), new Date().getDate());
  maxDob = new Date();

  patientForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    dob: [null, Validators.required],
    gender: ['Male' as Gender, Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    password: ['', Validators.required, Validators.minLength(8)],
  });

  doctorForm = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    dateOfJoining: [null, Validators.required],
    role: ['Doctor' as EmployeeRole],
    doctorDetails: this.fb.nonNullable.group({
      specializationId: ['', Validators.required],
      dob: [null, Validators.required],
      firstPracticeDate: [null, Validators.required],
      fee: [0, Validators.required],
      phone: ['', Validators.required],
    })
  });

  onSubmit() {
    if (this.userType === 'patient') {
      if (this.patientForm.invalid) return;
      const raw = this.patientForm.getRawValue();

      const payload: PatientRegisterForm = {
        ...raw,
        dob: raw.dob!,
      };
      this.submitted.emit(payload);
    }

    if (this.userType === 'doctor') {
      if (this.doctorForm.invalid) return;
      const raw = this.doctorForm.getRawValue();

      const payload: DoctorRegisterForm = {
        ...raw,
        dateOfJoining: raw.dateOfJoining!, 
        doctorDetails: {
          ...raw.doctorDetails,
          dob: raw.doctorDetails.dob!,
          firstPracticeDate: raw.doctorDetails.firstPracticeDate!
        },
      };

      this.submitted.emit(payload);
    }
  }
}
