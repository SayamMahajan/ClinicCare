import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../ui/material.module';
import { Payment } from '../../models/payment.model';

export interface PaymentDetailsDialogData {
  payment: Payment;
}

@Component({
  selector: 'payment-details-dialog',
  imports: [CommonModule, MaterialModule],
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
