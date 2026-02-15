import { Component, EventEmitter, inject, Output } from '@angular/core';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  ValidatorFn,
  AbstractControl,
  ValidationErrors,
} from '@angular/forms';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { Gender, PatientRegisterDto } from '../../../../shared/models/patient.model';

@Component({
  selector: 'app-register-form',
  imports: [ReactiveFormsModule, MaterialModule],
  templateUrl: './register-form.component.html',
})
export class RegisterFormComponent {
  @Output() submitted = new EventEmitter<PatientRegisterDto>();

  private fb = inject(FormBuilder);

  minDob = new Date(new Date().getFullYear() - 120, 0, 1); 
  maxDob = new Date(); 

  form = this.fb.nonNullable.group({
    firstName: ['', [Validators.required, Validators.maxLength(50)]],
    lastName: ['', [Validators.required, Validators.maxLength(50)]],
    dob: [null as Date | null, Validators.required],
    gender: ['Male' as Gender, Validators.required],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
    phone: ['', [Validators.required, Validators.minLength(10), Validators.pattern(/^\d{10}$/)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)]],
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const raw = this.form.getRawValue();

    const dob = raw.dob!;
    const formattedDob = `${dob.getFullYear()}-${String(dob.getMonth() + 1).padStart(
      2,
      '0'
    )}-${String(dob.getDate()).padStart(2, '0')}`;

    const payload: PatientRegisterDto = {
      firstName: raw.firstName,
      lastName: raw.lastName,
      dob: formattedDob,
      gender: raw.gender,
      email: raw.email,
      phone: raw.phone,
      password: raw.password,
    };

    this.submitted.emit(payload);
  }
}