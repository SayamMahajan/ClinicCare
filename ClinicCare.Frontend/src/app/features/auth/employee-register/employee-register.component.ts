import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-employee-register',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './employee-register.component.html',
  styleUrl: './employee-register.component.css',
})
export class EmployeeRegisterComponent {
  private fb = inject(FormBuilder);

  form = this.fb.group({
    role: ['Admin', Validators.required],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required],
    dateOfJoining: [null as Date | null, Validators.required],

    doctorDetails: this.fb.group({
      specializationId: [{ value: '', disabled: true }, Validators.required],
      dob: [null as Date | null, Validators.required],
      firstPracticeDate: [null as Date | null, Validators.required],
      fee: [{ value: '', disabled: true }, Validators.required],
      phone: ['', Validators.required],
    })
  });

  constructor() {
    this.form.get('role')?.valueChanges.subscribe((role) => {
      const doctorGroup = this.form.get('doctorDetails')!;
      role === 'Doctor'
        ? doctorGroup.enable()
        : doctorGroup.disable();
    });
  }

  submit() {
    if (this.form.invalid) return;

    // POST /api/employees/register
    console.log('Employee Register', this.form.getRawValue());
  }
}
