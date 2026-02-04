import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../shared/ui/material.module';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { TimeSlot } from '../../../shared/models/appointment.model';
import { DropdownFilterComponent } from '../../../shared/components/dropdown-filter/dropdown-filter.component';
import { DoctorResponseDto, EmployeeService } from '../../../shared/services/employee.service';
import { Subject, takeUntil } from 'rxjs';
import { AppointmentService } from '../../../shared/services/appointment.service';
import { AuthService } from '../../../shared/services/auth.service';
import { ConfirmDialogData } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';


@Component({
  selector: 'app-book-appointment',
  imports: [
    CommonModule,
    MaterialModule,
    ListFilterBarComponent,
    ListContainerComponent,
    ListRowComponent,
    DropdownFilterComponent
  ],
  templateUrl: './book-appointment.component.html'
})
export class BookAppointmentComponent implements OnInit, OnDestroy {
  private dialog = inject(MatDialog);
  private employeeService = inject(EmployeeService);
  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);
  private destroy$ = new Subject<void>();

  patientId = this.authService.userId || '';

  search = signal('');
  selectedSpecialization = signal<'All' | string>('All');

  selectedDoctor = signal<DoctorResponseDto | null>(null);
  selectedDate = signal<Date | null>(null);
  selectedSlot = signal<TimeSlot | null>(null);

  doctors = signal<DoctorResponseDto[]>([]);
  loadingDoctors = signal(false);
  submitting = signal(false);

  ngOnInit() {
    this.loadDoctors();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDoctors(specializationId?: string) {
    this.loadingDoctors.set(true);

    this.employeeService
      .getDoctors(specializationId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data: DoctorResponseDto[]) => {
          this.doctors.set(data);
          this.loadingDoctors.set(false);
        },
        error: (err) => {
          console.error('Failed to load doctors', err);
          this.doctors.set([]);
          this.loadingDoctors.set(false);
        }
      });
  }

  onSpecializationChange(value: string) {
    this.selectedSpecialization.set(value);
    this.selectedDoctor.set(null);

    value === 'All'
      ? this.loadDoctors()
      : this.loadDoctors(value);
  }

  specializations = computed(() => [
    'All',
    ...new Set(
      this.doctors()
        .map(d => d.specializationId)
        .filter(Boolean) as string[]
    )
  ]);

  filteredDoctors = computed(() =>
    this.doctors().filter(d =>
      `${d.firstName} ${d.lastName}`
        .toLowerCase()
        .includes(this.search().toLowerCase())
    )
  );

  onSlotChange(value: string) {
    this.selectedSlot.set(value as TimeSlot);
  }

  isSelected(doc: DoctorResponseDto): boolean {
    return this.selectedDoctor()?.id === doc.id;
  }

  toggleDoctor(doc: DoctorResponseDto) {
    this.selectedDoctor.set(
      this.isSelected(doc) ? null : doc
    );
  }

  getExperienceYears(dateOfJoining?: string): string {
    if (!dateOfJoining) return '-';

    const start = new Date(dateOfJoining);
    const now = new Date();

    let years = now.getFullYear() - start.getFullYear();
    if (
      now.getMonth() < start.getMonth() ||
      (now.getMonth() === start.getMonth() && now.getDate() < start.getDate())
    ) {
      years--;
    }

    return `${years} yrs`;
  }

  submit() {
    if (!this.selectedDoctor() || !this.selectedDate() || !this.selectedSlot()) {
      return;
    }

    const doctor = this.selectedDoctor()!;

    const dialogData: ConfirmDialogData = {
      title: 'Confirm Appointment',
      message: 'Please review your appointment details before confirming.',
      details: [
        { label: 'Doctor', value: `${doctor.firstName} ${doctor.lastName}` },
        { label: 'Specialization', value: doctor.specializationId ?? '-' },
        { label: 'Date', value: this.selectedDate()!.toDateString() },
        { label: 'Time Slot', value: this.selectedSlot()! },
        ...(doctor.fee
          ? [{ label: 'Consultation Fee', value: `₹${doctor.fee}` }]
          : [])
      ],
      confirmText: 'Book Appointment',
      cancelText: 'Edit'
    };

    this.dialog
      .open(ConfirmDialogComponent, {
        width: '420px',
        disableClose: true,
        data: dialogData
      })
      .afterClosed()
      .subscribe(confirmed => {
        if (confirmed) {
          this.createAppointment();
        }
      });
  }
  createAppointment() {
    const payload = {
      patientId: this.patientId,
      doctorId: this.selectedDoctor()!.id,
      date: this.selectedDate(),
      timeSlot: this.selectedSlot()
    };
    this.submitting.set(true);

    this.appointmentService
      .create(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (appointmentId) => {
          this.submitting.set(false);
          console.log('Appointment created:', appointmentId);

          this.selectedDoctor.set(null);
          this.selectedDate.set(null);
          this.selectedSlot.set(null);
        },
        error: (err) => {
          console.error('Appointment creation failed', err);
          this.submitting.set(false);
        }
      });
  }
}


  

