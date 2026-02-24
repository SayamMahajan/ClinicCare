import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { MaterialModule } from '../../shared/ui/material.module';
import { ListContainerComponent } from '../../shared/components/list-container/list-container.component';
import { ListRowComponent } from '../../shared/components/list-row/list-row.component';
import { ListFilterBarComponent } from '../../shared/components/list-filter-bar/list-filter-bar.component';
import { PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { PrescriptionService } from '../../services/prescription.service';
import { AuthService } from '../../services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { Subject, takeUntil } from 'rxjs';
import {
  PrescriptionResponseDto,
  getMedicationsSummary,
} from '../../shared/models/prescription.model';
import { PrescriptionSearchParams } from '../../shared/models/pagination.model';
import { ViewPrescriptionDialogComponent } from '../../shared/components/view-prescription-dialog/view-prescription-dialog.component';
import { PrescriptionFormDialogComponent } from '../../shared/components/prescription-form-dialog/prescription-form-dialog.component';

@Component({
  selector: 'app-prescription-list',
  imports: [
    MaterialModule,
    ListContainerComponent,
    ListRowComponent,
    ListFilterBarComponent,
    PaginationComponent,
  ],
  templateUrl: './prescription.component.html',
})
export class PrescriptionListComponent implements OnInit, OnDestroy {
  private prescriptionService = inject(PrescriptionService);
  private authService = inject(AuthService);
  private dialog = inject(MatDialog);
  private destroy$ = new Subject<void>();

  role = signal<string>(this.authService.role || 'Patient');
  currentUserId = signal<string>(this.authService.userId || '');

  search = signal('');

  prescriptions = signal<PrescriptionResponseDto[]>([]);
  loading = signal(false);

  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasPrevious = signal(false);
  hasNext = signal(false);

  ngOnInit() {
    this.loadPrescriptions();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadPrescriptions() {
    this.loading.set(true);

    const params: PrescriptionSearchParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.search() || undefined,
    };

    if (this.role() === 'Patient') {
      params.patientId = this.currentUserId();
    } else if (this.role() === 'Doctor') {
      params.doctorId = this.currentUserId();
    }

    this.prescriptionService
      .getAll(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.prescriptions.set(result.items);
          this.totalPages.set(result.totalPages);
          this.hasPrevious.set(result.hasPreviousPage);
          this.hasNext.set(result.hasNextPage);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Failed to load prescriptions', err);
          this.prescriptions.set([]);
          this.loading.set(false);
        },
      });
  }

  columns = computed(() => {
    return this.role() === 'Doctor'
      ? ['Patient', 'Date Prescribed', 'Medications', 'Action']
      : ['Doctor', 'Date Prescribed', 'Medications', 'Action'];
  });

  onSearchChange(value: string) {
    this.search.set(value);
    this.currentPage.set(1); 
    this.loadPrescriptions();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadPrescriptions();
  }

  mapRow(prescription: PrescriptionResponseDto): string[] {
    const userName =
      this.role() === 'Doctor'
        ? `${prescription.patient.firstName} ${prescription.patient.lastName}`
        : `${prescription.doctor.firstName} ${prescription.doctor.lastName}`;

    const date = new Date(prescription.createdAt).toLocaleDateString('en-US', {
      month: 'short',
      day: 'numeric',
      year: 'numeric',
    });

    const medications = getMedicationsSummary(prescription);

    return [userName, date, medications];
  }

  onInfoClick(id: string) {
    const prescription = this.prescriptions().find((p) => p.id === id);
    if (!prescription) return;

    this.dialog.open(ViewPrescriptionDialogComponent, {
      width: '750px',
      data: prescription,
    });
  }

  onCreatePrescription() {
    if (this.role() !== 'Doctor') return;

    this.dialog
      .open(PrescriptionFormDialogComponent, {
        width: '700px',
        disableClose: true,
      })
      .afterClosed()
      .subscribe((created) => {
        if (created) {
          this.loadPrescriptions();
        }
      });
  }
}