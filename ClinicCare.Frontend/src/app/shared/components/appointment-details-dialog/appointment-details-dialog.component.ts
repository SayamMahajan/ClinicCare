import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule, Time } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';
import { AppointmentResponseDto, TimeSlot } from '../../models/appointment.model';
import { TIME_SLOT_LABEL } from '../../utils/time-slot.mapper';

export interface AppointmentDialogAction {
  type: 'delete' | 'addPrescription' | 'viewPrescription';
  label: string;
  color?: 'primary' | 'warn';
}

export interface AppointmentDetailsDialogData {
  appointment: AppointmentResponseDto;
  actions: AppointmentDialogAction[];
}

@Component({
  selector: 'app-appointment-details-dialog',
  standalone: true,
  imports: [CommonModule, MaterialModule],
  templateUrl: './appointment-details-dialog.component.html'
})
export class AppointmentDetailsDialogComponent {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AppointmentDetailsDialogData,
    private dialogRef: MatDialogRef<AppointmentDetailsDialogComponent>
  ) {}

  slotLabel(slot: TimeSlot): string {
    return TIME_SLOT_LABEL[slot];
  }

  action(type: AppointmentDialogAction['type']) {
    this.dialogRef.close(type);
  }

  close() {
    this.dialogRef.close();
  }
}
