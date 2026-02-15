import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { LoginFormValue } from '../../../../shared/models/auth.model';

@Component({
  selector: 'app-login-form',
  imports: [ReactiveFormsModule, MaterialModule],
  templateUrl: './login-form.component.html',
})
export class LoginFormComponent {
  @Output() submitted = new EventEmitter<LoginFormValue>();

  loading = signal(false);
  private fb = inject(FormBuilder);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
    password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)]],
  });

  submit() {
    if (this.form.invalid) return;
    this.submitted.emit(this.form.getRawValue());
  }
}