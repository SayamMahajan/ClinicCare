import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { MaterialModule } from '../../shared/ui/material.module';
import { AuthService } from '../../shared/services/auth.service';
import { PatientService } from '../../shared/services/patient.service';
import { PatientUpdate } from '../../shared/models/patient.model';

@Component({
  selector: 'app-my-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, MaterialModule],
  templateUrl: './myprofile.component.html',
})
export class MyProfileComponent implements OnInit {

  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private patientService = inject(PatientService);
  private location = inject(Location);

  role = signal(this.authService.role || 'Patient');

  form = this.fb.group({
    firstName: [''],
    lastName: [''],
    email: [{ value: '', disabled: true }],
    phone: [''],
    password: [''],

    emergencyContact: [''],
    bloodGroup: [''],
    allergies: [''],
    height: [],
    bodyWeight: [],
    address: [''],
  });

  ngOnInit() {
    if (this.role() === 'Patient') {
      this.loadPatientProfile();
    }
  }

  loadPatientProfile() {
    this.patientService.getMyProfile().subscribe(profile => {
      this.form.patchValue(profile);
    });
  }

  save() {
    const raw = this.form.getRawValue();

    const payload: PatientUpdate = {
      firstName: raw.firstName ?? '',
      lastName: raw.lastName ?? '',
      phone: raw.phone ?? '',
      address: raw.address ?? '',
      password: raw.password ?? '',
      emergencyContact: raw.emergencyContact ?? undefined,
      bloodGroup: raw.bloodGroup ?? undefined,
      allergies: raw.allergies ?? undefined,
      bodyWeight: raw.bodyWeight ?? undefined,
      height: raw.height ?? undefined,
    };

    this.patientService.updateMyProfile(payload).subscribe();
  }

  goBack() {
    this.location.back();
  }
}
