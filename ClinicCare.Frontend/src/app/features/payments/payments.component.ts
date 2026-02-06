import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/ui/material.module';
import { PaymentService } from '../../shared/services/payment.service';
import { Payment, Role } from '../../shared/models/payment.model';
import { ListContainerComponent } from '../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../shared/components/list-row/list-row.component';
import { AuthService } from '../../shared/services/auth.service';
import { PaymentDetailsDialogComponent } from '../../shared/components/payment-details-dialog/payment-details-dialog.component';
import { MatDialog } from '@angular/material/dialog';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [
    CommonModule,
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent
  ],
  templateUrl: './payments.component.html'
})
export class PaymentsComponent {
  private dialog = inject(MatDialog);
  private authService = inject(AuthService);
  private paymentService = inject(PaymentService);

  role = signal<Role>((this.authService.role as Role) ?? 'Patient');

  payments = signal<Payment[]>([]);
  search = signal('');

  ngOnInit() {
    this.paymentService.getPayments().subscribe({
      next: res => this.payments.set(res),
    });
  }

  filteredPayments = computed(() => {
    const text = this.search().toLowerCase();

    return this.payments().filter(p =>
      p.patient.firstName.toLowerCase().includes(text) ||
      p.patient.lastName.toLowerCase().includes(text) ||
      p.doctor.firstName.toLowerCase().includes(text) ||
      p.doctor.lastName.toLowerCase().includes(text)
    );
  });

  get columns(): string[] {
    switch (this.role()) {
      case 'Admin':
        return ['Patient', 'Doctor', 'Amount', 'Date', 'Action'];
      case 'Doctor':
        return ['Patient', 'Amount', 'Date', 'Action'];
      default:
        return ['Doctor', 'Amount', 'Date', 'Action'];
    }
  }

  mapRow(payment: Payment): string[] {
    const date = new Date(payment.createdAt).toLocaleString();
    const amount = `₹${payment.amount}`;

    if (this.role() === 'Admin') {
      return [
        `${payment.patient.firstName} ${payment.patient.lastName}`,
        `${payment.doctor.firstName} ${payment.doctor.lastName}`,
        amount,
        date
      ];
    }

    if (this.role() === 'Doctor') {
      return [
        `${payment.patient.firstName} ${payment.patient.lastName}`,
        amount,
        date
      ];
    }

    return [
      `${payment.doctor.firstName} ${payment.doctor.lastName}`,
      amount,
      date
    ];
  }

  onInfoClick(payment: Payment) {
    this.dialog.open(PaymentDetailsDialogComponent, {
    width: '450px',
    data: {
      payment
    }
  });
  }
}
