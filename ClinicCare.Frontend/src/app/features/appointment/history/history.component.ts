import { Component, Input, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentResponseDto, AppointmentStatus, TimeSlot, PastTimeRange, UserRole } from '../../../shared/models/appointment.model';
import { TIME_SLOT_LABEL } from '../../../shared/utils/time-slot.mapper';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { MaterialModule } from '../../../shared/ui/material.module';
import { DropdownFilterComponent } from "../../../shared/components/dropdown-filter/dropdown-filter.component";

@Component({
  selector: 'app-history',
  imports: [
    CommonModule,
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent,
    DropdownFilterComponent
],
  templateUrl: './history.component.html',
})
export class HistoryComponent {
  /* ---------------- Role ---------------- */
  // @Input({ required: true }) role!: UserRole;
  role = signal<UserRole>('Doctor');

  /* ---------------- Filters ---------------- */
  search = signal('');
  selectedStatus = signal<'All' | AppointmentStatus>('All');
  selectedSlot = signal<'All' | TimeSlot>('All');
  selectedRange = signal<PastTimeRange>('All');

  /* ---------------- Mock Data (API later) ---------------- */
  appointments = signal<AppointmentResponseDto[]>([
    {
      id: crypto.randomUUID(),
      status: 'Completed',
      date: '2026-01-10',
      timeSlot: 'Morning',
      patient: { id: 'p1', firstName: 'Rahul', lastName: 'Sharma' },
      doctor: { id: 'd1', firstName: 'Amit', lastName: 'Singh' }
    },
    {
      id: crypto.randomUUID(),
      status: 'Cancelled',
      date: '2026-01-15',
      timeSlot: 'Evening',
      patient: { id: 'p2', firstName: 'Anita', lastName: 'Verma' },
      doctor: { id: 'd1', firstName: 'Amit', lastName: 'Singh' }
    }
  ]);

  /* ---------------- Computed Columns ---------------- */
  columns = computed(() => [
    this.role() === 'Doctor' ? 'Patient' : 'Doctor',
    'Date',
    'Slot',
    'Status'
  ]);

  /* ---------------- Filtering Logic ---------------- */
  filteredAppointments = computed(() => {
    const today = new Date();

    return this.appointments().filter(a => {

      /* Name filter */
      const name =
        this.role() === 'Doctor'
          ? `${a.patient.firstName} ${a.patient.lastName}`
          : `${a.doctor.firstName} ${a.doctor.lastName}`;

      const matchesSearch =
        name.toLowerCase().includes(this.search().toLowerCase());

      /* Slot filter */
      const matchesSlot =
        this.selectedSlot() === 'All' ||
        a.timeSlot === this.selectedSlot();

      /* Status filter */
      const matchesStatus =
        this.selectedStatus() === 'All' ||
        a.status === this.selectedStatus();

      /* Past time range filter */
      const appointmentDate = new Date(a.date);
      const diffDays =
        (today.getTime() - appointmentDate.getTime()) /
        (1000 * 60 * 60 * 24);

      let matchesRange = true;

      switch (this.selectedRange()) {
        case 'Past Day':
          matchesRange = diffDays <= 1;
          break;
        case 'Past Week':
          matchesRange = diffDays > 1 && diffDays <= 7;
          break;
        case 'Past Month':
          matchesRange = diffDays > 7 && diffDays <= 30;
          break;
      }

      return (
        matchesSearch &&
        matchesSlot &&
        matchesStatus &&
        matchesRange
      );
    });
  });

  getCounterpartyName(a: AppointmentResponseDto): string {
    return this.role() === 'Doctor'
      ? `${a.patient.firstName} ${a.patient.lastName}`
      : `${a.doctor.firstName} ${a.doctor.lastName}`;
  }

  getSlotLabel(slot: TimeSlot): string {
    return TIME_SLOT_LABEL[slot];
  }

  onRangeChange(value: string) {
    this.selectedRange.set(value as PastTimeRange);
  }

  onSlotChange(value: string) {
    this.selectedSlot.set(value as 'All' | TimeSlot);
  }

  onStatusChange(value: string) {
    this.selectedStatus.set(value as 'All' | AppointmentStatus);
  }

  onInfo(id: string) {
    console.log('History appointment:', id);
  }
}