import { Component, EventEmitter, inject, Input, Output, signal } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MaterialModule } from '../../../../shared/ui/material.module';
import { AuthUser, LoginFormValue } from '../../../../shared/models/auth.model';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-login-form',
  imports: [ReactiveFormsModule, MaterialModule, RouterLink],
  templateUrl: './login-form.component.html',
})
export class LoginFormComponent {
  @Input({ required: true }) userType!: AuthUser;
  @Output() submitted = new EventEmitter<LoginFormValue>();

  loading = signal(false);
  private fb = inject(FormBuilder);

  form = this.fb.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(100), Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)]],
    password: ['', [Validators.required, Validators.minLength(8)]],
  });

  submit() {
    if (this.form.invalid) return;
    this.submitted.emit(this.form.getRawValue());
  }
}