import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentResponseDto, TimeSlot, FutureTimeRange, UserRole} from '../../../shared/models/appointment.model';
import { TIME_SLOT_LABEL } from '../../../shared/utils/time-slot.mapper';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { MaterialModule } from '../../../shared/ui/material.module';
import { DropdownFilterComponent } from "../../../shared/components/dropdown-filter/dropdown-filter.component";
import { Subject, takeUntil } from 'rxjs';
import { AppointmentService } from '../../../shared/services/appointment.service';
import { AuthService } from '../../../shared/services/auth.service';
import { MatDialog } from '@angular/material/dialog';
import { AppointmentDetailsDialogComponent } from '../../../shared/components/appointment-details-dialog/appointment-details-dialog.component';
import { resolveAppointmentActions } from '../../../shared/utils/appointment-action';
import { PrescriptionFormDialogComponent } from '../../../shared/components/prescription-form-dialog/prescription-form-dialog.component';
import { PrescriptionService } from '../../../shared/services/prescription.service';
import { ViewPrescriptionDialogComponent } from '../../../shared/components/view-prescription-dialog/view-prescription-dialog.component';


@Component({
  selector: 'app-scheduled',
  imports: [
    CommonModule,
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent,
    DropdownFilterComponent
],
  templateUrl: './scheduled.component.html'
})
export class ScheduledComponent implements OnInit, OnDestroy {

  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);
  private prescriptionService = inject(PrescriptionService);
  private dialog = inject(MatDialog);
  private destroy$ = new Subject<void>();

  role = signal<UserRole>(this.authService.role as UserRole || 'Patient');

  search = signal('');
  selectedSlot = signal<'All' | TimeSlot>('All');
  selectedRange = signal<FutureTimeRange>('All');

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

    this.appointmentService
      .getAppointments('Approved')
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: data => {
          this.appointments.set(data);
          this.loading.set(false);
        },
        error: err => {
          console.error('Failed to load scheduled appointments', err);
          this.appointments.set([]);
          this.loading.set(false);
        }
      });
  }

  columns = computed(() => [
    this.role() === 'Doctor' ? 'Patient' : 'Doctor',
    'Date',
    'Slot',
    'Action'
  ]);

  filteredAppointments = computed(() => {
    const today = new Date();

    return this.appointments().filter(a => {

      const appointmentDate = new Date(a.date);
      if (appointmentDate < today) return false;

      const name =
        this.role() === 'Doctor'
          ? `${a.patient.firstName} ${a.patient.lastName}`
          : `${a.doctor.firstName} ${a.doctor.lastName}`;

      const matchesSearch =
        name.toLowerCase().includes(this.search().toLowerCase());

      const matchesSlot =
        this.selectedSlot() === 'All' ||
        a.timeSlot === this.selectedSlot();

      const diffDays =
        (appointmentDate.getTime() - today.getTime()) /
        (1000 * 60 * 60 * 24);

      let matchesRange = true;

      switch (this.selectedRange()) {
        case 'Next Day':
          matchesRange = diffDays <= 1;
          break;
        case 'Next Week':
          matchesRange = diffDays > 1 && diffDays <= 7;
          break;
        case 'Next Month':
          matchesRange = diffDays > 7 && diffDays <= 30;
          break;
      }

      return matchesSearch && matchesSlot && matchesRange;
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

  onRangeChange(value: string) {
    this.selectedRange.set(value as FutureTimeRange);
  }

  onSlotChange(value: string) {
    this.selectedSlot.set(value as 'All' | TimeSlot);
  }

  onInfo(id: string) {
    const appointment = this.appointments().find(a => a.id === id);
    if (!appointment) return;

    const actions = resolveAppointmentActions(appointment, 'scheduled');

    this.dialog
    .open(AppointmentDetailsDialogComponent, {
      width: '450px',
      data: {
        appointment,
        actions
      }
    })
    .afterClosed()
    .subscribe(action => {
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
    const apt = this.appointments().find(a => a.id === appointmentId);
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
          doctorName: `${apt.doctor.firstName} ${apt.doctor.lastName}`
        }
      })
      .afterClosed()
      .subscribe(payload => {
        if (!payload) return;

        this.prescriptionService
          .createPrescription(payload)
          .subscribe(() => {
            this.loadAppointments();
          });
      });
  }

  viewPrescription(appointmentId: string) {
    const apt = this.appointments().find(a => a.id === appointmentId);
    if (!apt || !apt.prescriptionId) return;

    this.prescriptionService
      .getById(apt.prescriptionId)
      .subscribe(prescription => {
        if (!prescription) return;

        this.dialog.open(ViewPrescriptionDialogComponent, {
          width: '750px',
          data: prescription
        });
      });
  }
}