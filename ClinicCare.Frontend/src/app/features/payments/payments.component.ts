import { Component, inject, OnInit, signal } from '@angular/core';
import { MaterialModule } from '../../shared/ui/material.module';
import { PaymentResponseDto } from '../../shared/models/payment.model';
import { ListContainerComponent } from '../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../shared/components/list-row/list-row.component';
import { PaymentDetailsDialogComponent } from '../../shared/components/payment-details-dialog/payment-details-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { AuthService } from '../../services/auth.service';
import { PaymentService } from '../../services/payment.service';
import { LoaderService } from '../../services/loader.service';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';

type Role = 'Admin' | 'Doctor' | 'Patient';

@Component({
  selector: 'app-payments',
  imports: [
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent,
    PaginationComponent
  ],
  templateUrl: './payments.component.html'
})
export class PaymentsComponent implements OnInit {
  private dialog = inject(MatDialog);
  private authService = inject(AuthService);
  private paymentService = inject(PaymentService);

  role = signal<Role>((this.authService.role as Role) ?? 'Patient');
  payments = signal<PaymentResponseDto[]>([]);
  search = signal('');
  loading = signal(false);

  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasPreviousPage = signal(false);
  hasNextPage = signal(false);

  ngOnInit() {
    this.loadPayments();
  }

  loadPayments() {
    this.loading.set(true);

    const params = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.search() || undefined
    };

    this.paymentService.getAll(params).subscribe({
      next: result => {
        this.payments.set(result.items); 
        this.totalPages.set(result.totalPages);
        this.hasPreviousPage.set(result.hasPreviousPage);
        this.hasNextPage.set(result.hasNextPage);
        this.loading.set(false);
      },
      error: err => {
        console.error('Failed to load payments:', err);
        this.payments.set([]);
        this.loading.set(false);
      }
    });
  }

  get columns(): string[] {
    switch (this.role()) {
      case 'Admin':
        return ['Patient', 'Doctor', 'Amount', 'Transaction ID', 'Date', 'Action'];
      case 'Doctor':
        return ['Patient', 'Amount', 'Transaction ID', 'Date', 'Action'];
      default:
        return ['Doctor', 'Amount', 'Transaction ID', 'Date', 'Action'];
    }
  }

  mapRow(payment: PaymentResponseDto): string[] {
    const date = new Date(payment.createdAt).toLocaleDateString();
    const amount = `₹${payment.amount.toFixed(2)}`;

    if (this.role() === 'Admin') {
      return [
        `${payment.patient.firstName} ${payment.patient.lastName}`,
        `${payment.doctor.firstName} ${payment.doctor.lastName}`,
        amount,
        payment.transactionId,
        date
      ];
    }

    if (this.role() === 'Doctor') {
      return [
        `${payment.patient.firstName} ${payment.patient.lastName}`,
        amount,
        payment.transactionId,
        date
      ];
    }

    return [
      `${payment.doctor.firstName} ${payment.doctor.lastName}`,
      amount,
      payment.transactionId,
      date
    ];
  }

  onSearchChange(value: string) {
    this.search.set(value);
    this.currentPage.set(1);
    this.loadPayments();
  }

  onInfoClick(payment: PaymentResponseDto) {
    this.dialog.open(PaymentDetailsDialogComponent, {
      width: '450px',
      data: { payment }
    });
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadPayments();
  }
}