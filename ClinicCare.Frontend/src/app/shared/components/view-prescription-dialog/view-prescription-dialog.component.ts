import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';
import { PrescriptionResponseDto } from '../../models/prescription.model';

@Component({
  selector: 'app-view-prescription-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './view-prescription-dialog.component.html'
})
export class ViewPrescriptionDialogComponent {
  dialogRef = inject(MatDialogRef<ViewPrescriptionDialogComponent>);
  prescription = inject(MAT_DIALOG_DATA) as PrescriptionResponseDto;

  close() {
    this.dialogRef.close();
  }
}
