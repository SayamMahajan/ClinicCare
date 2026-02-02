import { Component, EventEmitter, inject, Output, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { LoginFormValue } from '../../../../shared/models/auth.models';

@Component({
  selector: 'app-login-form',
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './login-form.component.html'
})
export class LoginFormComponent {
  @Output() submitted = new EventEmitter<LoginFormValue>();

  loading = signal(false);
  private fb = inject(FormBuilder);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(8)]]
  });

  submit() {
    if (this.form.invalid) return;
    this.submitted.emit(this.form.getRawValue());
  }
}
