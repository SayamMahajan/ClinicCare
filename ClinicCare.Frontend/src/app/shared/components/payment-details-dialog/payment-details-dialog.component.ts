import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { MaterialModule } from '../../ui/material.module';
import { PaymentResponseDto } from '../../models/payment.model';
import { DatePipe } from '@angular/common';

export interface PaymentDetailsDialogData {
  payment: PaymentResponseDto;
}

@Component({
  selector: 'payment-details-dialog',
  imports: [MaterialModule, DatePipe],
  templateUrl: './payment-details-dialog.component.html'
})
export class PaymentDetailsDialogComponent {

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: PaymentDetailsDialogData,
    private dialogRef: MatDialogRef<PaymentDetailsDialogComponent>
  ) {}

  close() {
    this.dialogRef.close();
  }
}
