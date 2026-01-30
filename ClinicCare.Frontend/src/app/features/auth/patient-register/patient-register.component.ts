import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatRadioModule } from '@angular/material/radio';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

@Component({
  selector: 'app-patient-register',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatRadioModule,
    MatDatepickerModule,
    MatNativeDateModule
  ],
  templateUrl: './patient-register.component.html',
  styleUrl: './patient-register.component.css',
})

export class PatientRegisterComponent {
  private fb = inject(FormBuilder);

  form = this.fb.nonNullable.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    phone: ['', Validators.required],
    dob: [null as Date | null, Validators.required],
    gender: ['', Validators.required],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  submit() {
    if (this.form.invalid) return;

    // POST /api/patients/register
    console.log('Patient Register', this.form.getRawValue());
  }
}
