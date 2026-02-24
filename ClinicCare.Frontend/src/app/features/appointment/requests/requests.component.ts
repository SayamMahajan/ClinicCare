import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { AppointmentResponseDto, TimeSlot } from '../../../shared/models/appointment.model';
import { TIME_SLOT_LABEL } from '../../../shared/utils/time-slot.mapper';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { MaterialModule } from '../../../shared/ui/material.module';
import { DropdownFilterComponent } from '../../../shared/components/dropdown-filter/dropdown-filter.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { Subject, takeUntil } from 'rxjs';
import { MatDialog } from '@angular/material/dialog';
import { AppointmentDetailsDialogComponent } from '../../../shared/components/appointment-details-dialog/appointment-details-dialog.component';
import { resolveAppointmentActions } from '../../../shared/utils/appointment-action';
import { AppointmentService } from '../../../services/appointment.service';
import { AuthService } from '../../../services/auth.service';
import { AppointmentSearchParams } from '../../../shared/models/pagination.model';

@Component({
  selector: 'app-requests',
  imports: [
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent,
    DropdownFilterComponent,
    PaginationComponent,
  ],
  templateUrl: './requests.component.html',
})
export class RequestsComponent implements OnInit, OnDestroy {
  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);
  private dialog = inject(MatDialog);
  private destroy$ = new Subject<void>();

  // User role for display logic
  role = signal<string>(this.authService.role || 'Patient');

  // Search and filters
  search = signal('');
  selectedSlot = signal<'All' | TimeSlot>('All');

  // Pagination
  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasPrevious = signal(false);
  hasNext = signal(false);

  // Data
  appointments = signal<AppointmentResponseDto[]>([]);
  loading = signal(false);

  ngOnInit() {
    this.loadAppointments();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadAppointments() {
    this.loading.set(true);

    const params: AppointmentSearchParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      searchTerm: this.search() || undefined,
      status: 'Requested',
    };

    this.appointmentService
      .getAll(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.appointments.set(result.items);
          this.totalPages.set(result.totalPages);
          this.hasPrevious.set(result.hasPreviousPage);
          this.hasNext.set(result.hasNextPage);
          this.loading.set(false);
        },
        error: (err) => {
          console.error('Failed to load appointment requests', err);
          this.appointments.set([]);
          this.loading.set(false);
        },
      });
  }

  columns = computed(() => [
    this.role() === 'Doctor' ? 'Patient' : 'Doctor',
    'Date',
    'Slot',
    'Action',
  ]);

  filteredAppointments = computed(() => {
    return this.appointments().filter((a) => {
      const matchesSlot =
        this.selectedSlot() === 'All' || a.timeSlot === this.selectedSlot();
      return matchesSlot;
    });
  });

  getUserName(a: AppointmentResponseDto): string {
    return this.role() === 'Doctor'
      ? `${a.patient.firstName} ${a.patient.lastName}`
      : `${a.doctor.firstName} ${a.doctor.lastName}`;
  }

  getSlotLabel(slot: TimeSlot): string {
    return TIME_SLOT_LABEL[slot];
  }

  onSearchChange(value: string) {
    this.search.set(value);
    this.currentPage.set(1);
    this.loadAppointments();
  }

  onSlotChange(value: string) {
    this.selectedSlot.set(value as 'All' | TimeSlot);
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadAppointments();
  }

  onInfo(id: string) {
    const appointment = this.appointments().find((a) => a.id === id);
    if (!appointment) return;

    const actions = resolveAppointmentActions(appointment, 'requested');

    this.dialog
      .open(AppointmentDetailsDialogComponent, {
        width: '450px',
        data: {
          appointment,
          actions,
        },
      })
      .afterClosed()
      .subscribe((action) => {
        switch (action) {
          case 'approve':
            this.approveAppointment(id);
            break;
          case 'delete':
            this.deleteAppointment(id);
            break;
        }
      });
  }

  approveAppointment(id: string) {
    this.appointmentService.patch(id, { status: 'Approved' }).subscribe(() => {
      this.loadAppointments();
    });
  }

  deleteAppointment(id: string) {
    this.appointmentService.delete(id).subscribe(() => {
      this.loadAppointments();
    });
  }
}