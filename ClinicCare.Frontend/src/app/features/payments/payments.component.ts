import { Component, Input, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../shared/ui/material.module';
import { PaymentService } from '../../shared/services/payment.service';
import { Payment, Role } from '../../shared/models/payment.model';
import { ListContainerComponent } from '../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../shared/components/list-row/list-row.component';

@Component({
  selector: 'app-payments',
  imports: [CommonModule, MaterialModule,ListContainerComponent, ListFilterBarComponent, ListRowComponent],
  templateUrl: './payments.component.html'
})
export class PaymentsComponent {
  @Input({ required: true }) role!: Role;

  payments = signal<Payment[]>([]);
  search = signal('');

  private paymentService = inject(PaymentService);

  ngOnInit() {
    this.paymentService.getPayments(this.role).subscribe({
      next: res => {
        this.payments.set(res);
      },
    });
  }

  filteredPayments = computed(() => {
    return this.payments().filter(p => {
      const searchText = this.search().toLowerCase();

      return (
        p.patient.firstName.toLowerCase().includes(searchText) ||
        p.patient.lastName.toLowerCase().includes(searchText) ||
        p.doctor.firstName.toLowerCase().includes(searchText) ||
        p.doctor.lastName.toLowerCase().includes(searchText)
      );
    });
  });

  get columns(): string[] {
    if (this.role === 'Admin') {
      return ['Patient', 'Doctor', 'Amount', 'Date'];
    }
    if (this.role === 'Doctor') {
      return ['Patient', 'Amount', 'Date'];
    }
    return ['Doctor', 'Amount', 'Date'];
  }

  mapRow(payment: Payment): string[] {
    if (this.role === 'Admin') {
      return [
        `${payment.patient.firstName} ${payment.patient.lastName}`,
        `${payment.doctor.firstName} ${payment.doctor.lastName}`,
        `₹${payment.amount}`,
        new Date(payment.createdAt).toLocaleString()
      ];
    }

    if (this.role === 'Doctor') {
      return [
        `${payment.patient.firstName} ${payment.patient.lastName}`,
        `₹${payment.amount}`,
        new Date(payment.createdAt).toLocaleString()
      ];
    }

    return [
      `${payment.doctor.firstName} ${payment.doctor.lastName}`,
      `₹${payment.amount}`,
      new Date(payment.createdAt).toLocaleString()
    ];
  }
}
