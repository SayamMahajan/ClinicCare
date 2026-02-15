import { Component, computed, inject, OnDestroy, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { MaterialModule } from '../../../shared/ui/material.module';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { AppointmentCreateDto, TimeSlot } from '../../../shared/models/appointment.model';
import { DropdownFilterComponent } from '../../../shared/components/dropdown-filter/dropdown-filter.component';
import { Subject, takeUntil } from 'rxjs';
import { ConfirmDialogData, ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { EmployeeResponseDto } from '../../../shared/models/employee.model';
import { SpecializationResponseDto } from '../../../shared/models/specialization.model';
import { EmployeeService } from '../../../services/employee.service';
import { AppointmentService } from '../../../services/appointment.service';
import { SpecializationService } from '../../../services/specialization.service';
import { AuthService } from '../../../services/auth.service';
import { EmployeeSearchParams } from '../../../shared/models/pagination.model';

@Component({
  selector: 'app-book-appointment',
  imports: [
    MaterialModule,
    ListFilterBarComponent,
    DropdownFilterComponent,
    PaginationComponent,
  ],
  templateUrl: './book-appointment.component.html',
})
export class BookAppointmentComponent implements OnInit, OnDestroy {
  private dialog = inject(MatDialog);
  private employeeService = inject(EmployeeService);
  private appointmentService = inject(AppointmentService);
  private authService = inject(AuthService);
  private specializationService = inject(SpecializationService);
  private router = inject(Router);
  private snackBar = inject(MatSnackBar);
  private destroy$ = new Subject<void>();

  patientId = this.authService.userId || '';

  specializations = signal<SpecializationResponseDto[]>([]);
  specializationMap = signal<Map<string, string>>(new Map());

  search = signal('');
  selectedSpecializationId = signal<string>('All');

  selectedDoctor = signal<EmployeeResponseDto | null>(null);
  selectedDate = signal<Date | null>(null);
  selectedSlot = signal<TimeSlot | null>(null);

  doctors = signal<EmployeeResponseDto[]>([]);
  loadingDoctors = signal(false);
  submitting = signal(false);

  currentPage = signal(1);
  pageSize = signal(10);
  totalPages = signal(0);
  hasPrevious = signal(false);
  hasNext = signal(false);

  minDate = new Date();

  ngOnInit() {
    this.loadSpecializations();
    this.loadDoctors();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadSpecializations() {
    this.specializationService
      .getAll({ pageNumber: 1, pageSize: 100 })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.specializations.set(result.items);
          const map = new Map<string, string>();
          result.items.forEach((spec) => {
            map.set(spec.id, spec.type);
          });
          this.specializationMap.set(map);
        },
        error: (err) => {
          console.error('Failed to load specializations:', err);
        },
      });
  }

  loadDoctors() {
    this.loadingDoctors.set(true);

    const params: EmployeeSearchParams = {
      pageNumber: this.currentPage(),
      pageSize: this.pageSize(),
      role: 'Doctor',
      searchTerm: this.search() || undefined,
      specializationId:
        this.selectedSpecializationId() !== 'All'
          ? this.selectedSpecializationId()
          : undefined,
    };

    this.employeeService
      .getAll(params)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (result) => {
          this.doctors.set(result.items);
          this.totalPages.set(result.totalPages);
          this.hasPrevious.set(result.hasPreviousPage);
          this.hasNext.set(result.hasNextPage);
          this.loadingDoctors.set(false);
        },
        error: (err) => {
          console.error('Failed to load doctors', err);
          this.doctors.set([]);
          this.loadingDoctors.set(false);
        },
      });
  }

  specializationOptions = computed(() => {
    return ['All', ...Array.from(this.specializationMap().values())];
  });

  getSpecializationName(id?: string): string {
    if (!id) return '-';
    return this.specializationMap().get(id) ?? '-';
  }

  getExperienceYears(firstPracticeDate?: string): string {
    if (!firstPracticeDate) return '-';

    const start = new Date(firstPracticeDate);
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

  onSpecializationChange(value: string) {
    let specializationId = 'All';
    if (value !== 'All') {
      for (const [id, name] of this.specializationMap().entries()) {
        if (name === value) {
          specializationId = id;
          break;
        }
      }
    }

    this.selectedSpecializationId.set(specializationId);
    this.currentPage.set(1); 
    this.selectedDoctor.set(null); 
    this.loadDoctors();
  }

  onSearchChange(value: string) {
    this.search.set(value);
    this.currentPage.set(1);
    this.loadDoctors();
  }

  onPageChange(page: number) {
    this.currentPage.set(page);
    this.loadDoctors();
  }

  onPageSizeChange(size: number) {
    this.pageSize.set(size);
    this.currentPage.set(1);
    this.loadDoctors();
  }

  onDateChange(event: MatDatepickerInputEvent<Date>) {
    this.selectedDate.set(event.value);
  }

  onSlotChange(value: string) {
    this.selectedSlot.set(value as TimeSlot);
  }

  isSelected(doc: EmployeeResponseDto): boolean {
    return this.selectedDoctor()?.id === doc.id;
  }

  toggleDoctor(doc: EmployeeResponseDto) {
    this.selectedDoctor.set(this.isSelected(doc) ? null : doc);
  }

  canSubmit = computed(() => {
    return (
      this.selectedDoctor() !== null &&
      this.selectedDate() !== null &&
      this.selectedSlot() !== null
    );
  });

  submit() {
    if (!this.canSubmit()) {
      this.showError('Please select doctor, date, and time slot');
      return;
    }

    const doctor = this.selectedDoctor()!;

    const dialogData: ConfirmDialogData = {
      title: 'Confirm Appointment',
      message: 'Please review your appointment details before confirming.',
      details: [
        { label: 'Doctor', value: `Dr. ${doctor.firstName} ${doctor.lastName}` },
        {
          label: 'Specialization',
          value: this.getSpecializationName(doctor.specializationId),
        },
        { label: 'Date', value: this.selectedDate()!.toDateString() },
        { label: 'Time Slot', value: this.selectedSlot()! },
        ...(doctor.fee ? [{ label: 'Consultation Fee', value: `$${doctor.fee}` }] : []),
      ],
      confirmText: 'Book Appointment',
      cancelText: 'Edit',
    };

    this.dialog
      .open(ConfirmDialogComponent, {
        width: '420px',
        disableClose: true,
        data: dialogData,
      })
      .afterClosed()
      .subscribe((confirmed) => {
        if (confirmed) {
          this.createAppointment();
        }
      });
  }

  createAppointment() {
    const payload: AppointmentCreateDto = {
      patientId: this.patientId,
      doctorId: this.selectedDoctor()!.id,
      date: this.formatDate(this.selectedDate()!),
      paymentId: '', 
      timeSlot: this.selectedSlot()!,
    };

    this.submitting.set(true);

    this.appointmentService
      .create(payload)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.submitting.set(false);
          this.showSuccess('Appointment booked successfully!');

          this.selectedDoctor.set(null);
          this.selectedDate.set(null);
          this.selectedSlot.set(null);

          setTimeout(() => {
            this.router.navigate(['/appointments/scheduled']);
          }, 1500);
        },
        error: (err) => {
          console.error('Appointment creation failed', err);
          this.showError('Failed to book appointment. Please try again.');
          this.submitting.set(false);
        },
      });
  }

  private formatDate(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  }

  private showSuccess(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 3000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['success-snackbar'],
    });
  }

  private showError(message: string) {
    this.snackBar.open(message, 'Close', {
      duration: 5000,
      horizontalPosition: 'end',
      verticalPosition: 'top',
      panelClass: ['error-snackbar'],
    });
  }
}