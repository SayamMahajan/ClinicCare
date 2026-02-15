import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../../shared/ui/material.module';
import { EmployeeResponseDto, EmployeeDialogAction } from '../../models/employee.model';

export interface EmployeeDetailsDialogData {
  employee: EmployeeResponseDto;
}

@Component({
  selector: 'app-employee-details-dialog',
  imports: [MaterialModule],
  templateUrl: './employee-details-dialog.component.html'
})
export class EmployeeDetailsDialogComponent {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: EmployeeDetailsDialogData,
    private dialogRef: MatDialogRef<EmployeeDetailsDialogComponent>
  ) {}

  action(type: EmployeeDialogAction) {
    this.dialogRef.close(type);
  }

  close() {
    this.dialogRef.close();
  }
}

