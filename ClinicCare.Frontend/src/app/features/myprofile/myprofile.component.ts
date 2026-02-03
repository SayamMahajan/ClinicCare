import { CommonModule } from '@angular/common';
import { Component, computed, inject } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../shared/ui/material.module';
import { AuthService } from '../../shared/services/auth.service';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './myprofile.component.html'
})
export class MyProfileComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);

  role = 'Patient' // 'Patient' | 'Doctor' | 'Admin'

  canEdit = computed(() =>
    this.role === 'Patient' || this.role === 'Admin'
  );

  form = this.fb.group({
    // Common
    firstName: [''],
    lastName: [''],
    email: [{ value: '', disabled: true }],
    phone: [''],

    // Patient-only
    emergencyContact: [''],
    bloodGroup: [''],
    allergies: [''],
    bodyWeight: [''],
    height: [''],
    address: [''],

    // Doctor/Admin
    fee: [''],
    specializationId: [''],
  });

  constructor(
    
  ) {}

  // ngOnInit() {
  //   this.loadProfile();

  //   if (!this.canEdit()) {
  //     this.form.disable();
  //   }
  // }

  // loadProfile() {
  //   this.profileApi.getMyProfile().subscribe(profile => {
  //     this.form.patchValue(profile);
  //   });
  // }

  // submit() {
  //   if (!this.form.valid) return;

  //   if (this.role === 'Patient') {
  //     this.profileApi.updatePatient(this.form.value).subscribe();
  //   }

  //   if (this.role === 'Admin') {
  //     this.profileApi.updateEmployee(this.form.value).subscribe();
  //   }
  // }
}
