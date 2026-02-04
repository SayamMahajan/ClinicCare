import { Component, inject, Inject, Input, input } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';
import { MedicationDto, PrescriptionCreateDto, PrescriptionDialogData } from '../../models/prescription.model';

@Component({
  selector: 'app-prescription-form-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule, ReactiveFormsModule],
  templateUrl: './prescription-form-dialog.component.html'
})
export class PrescriptionFormDialogComponent {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<PrescriptionFormDialogComponent>);
  data = inject(MAT_DIALOG_DATA) as PrescriptionDialogData;

  form = this.fb.group({
    description: this.fb.array([this.createMedication()])
  });
  
  get medications(): FormArray {
    return this.form.get('description') as FormArray;
  }

  createMedication(): FormGroup {
    return this.fb.group({
      medicine: ['', [Validators.required, Validators.maxLength(100)]],
      dosage: [1, [Validators.required, Validators.min(1), Validators.max(10)]],
      frequency: ['', [Validators.required, Validators.maxLength(50)]],
      days: [1, [Validators.required, Validators.min(1), Validators.max(365)]],
      instructions: ['', Validators.maxLength(500)],
    });
  }

  addMedication() {
    this.medications.push(this.createMedication());
  }

  removeMedication(index: number) {
    this.medications.removeAt(index);
  }

  submit() {
    if (this.form.invalid) return;

    const payload: PrescriptionCreateDto = {
      patientId: this.data.patientId,
      doctorId: this.data.doctorId,
      appointmentId: this.data.appointmentId,
      description: this.form.value.description as MedicationDto[]
    };

    this.dialogRef.close(payload);
  }

  cancel() {
    this.dialogRef.close();
  }
}
