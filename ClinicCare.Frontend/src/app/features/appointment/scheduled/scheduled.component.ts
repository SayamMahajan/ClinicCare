import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import {
  AppointmentResponseDto,
  TimeSlot,
} from '../../../shared/models/appointment.model';
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
import { PrescriptionFormDialogComponent } from '../../../shared/components/prescription-form-dialog/prescription-form-dialog.component';
import { ViewPrescriptionDialogComponent } from '../../../shared/components/view-prescription-dialog/view-prescription-dialog.component';
import { AppointmentService } from '../../../services/appointment.service';
import { PrescriptionService } from '../../../services/prescription.service';
import { AuthService } from '../../../services/auth.service';
import { AppointmentSearchParams } from '../../../shared/models/pagination.model';

@Component({
  selector: 'app-scheduled',
  imports: [
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent,
    DropdownFilterComponent,
    PaginationComponent,
  ],
  templateUrl: './scheduled.component.html',
})
export class ScheduledComponent implements OnInit, OnDestroy {
  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);
  private prescriptionService = inject(PrescriptionService);
  private dialog = inject(MatDialog);
  private destroy$ = new Subject<void>();

  role = signal<string>(this.authService.role || 'Patient');

  search = signal('');
  selectedSlot = signal<'All' | TimeSlot>('All');

  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasPrevious = signal(false);
  hasNext = signal(false);

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
      status: 'Approved',
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
          console.error('Failed to load scheduled appointments', err);
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
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    return this.appointments().filter((a) => {
      const appointmentDate = new Date(a.date);
      appointmentDate.setHours(0, 0, 0, 0);

      // Only future appointments
      if (appointmentDate < today) return false;

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

  onPageSizeChange(size: number) {
    this.pageSize.set(size);
    this.currentPage.set(1); 
    this.loadAppointments();
  }

  onInfo(id: string) {
    const appointment = this.appointments().find((a) => a.id === id);
    if (!appointment) return;

    const actions = resolveAppointmentActions(appointment, 'scheduled');

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
          case 'delete':
            this.deleteAppointment(id);
            break;
          case 'addPrescription':
            this.addPrescription(id);
            break;
          case 'viewPrescription':
            this.viewPrescription(id);
            break;
        }
      });
  }

  deleteAppointment(id: string) {
    this.appointmentService.delete(id).subscribe(() => {
      this.loadAppointments();
    });
  }

  addPrescription(appointmentId: string) {
    const apt = this.appointments().find((a) => a.id === appointmentId);
    if (!apt) return;

    this.dialog
      .open(PrescriptionFormDialogComponent, {
        width: '700px',
        disableClose: true,
        data: {
          patientId: apt.patient.id,
          doctorId: apt.doctor.id,
          appointmentId: apt.id,
          patientName: `${apt.patient.firstName} ${apt.patient.lastName}`,
          doctorName: `${apt.doctor.firstName} ${apt.doctor.lastName}`,
        },
      })
      .afterClosed()
      .subscribe((payload) => {
        if (!payload) return;

        this.prescriptionService.create(payload).subscribe(() => {
          this.loadAppointments();
        });
      });
  }

  viewPrescription(appointmentId: string) {
    const apt = this.appointments().find((a) => a.id === appointmentId);
    if (!apt || !apt.prescriptionId) return;

    this.prescriptionService.getById(apt.prescriptionId).subscribe((prescription) => {
      if (!prescription) return;

      this.dialog.open(ViewPrescriptionDialogComponent, {
        width: '750px',
        data: prescription,
      });
    });
  }
}