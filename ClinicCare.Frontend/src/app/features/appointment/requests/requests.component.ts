import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AppointmentResponseDto, TimeSlot, FutureTimeRange, UserRole } from '../../../shared/models/appointment.model';
import { TIME_SLOT_LABEL } from '../../../shared/utils/time-slot.mapper';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { MaterialModule } from '../../../shared/ui/material.module';
import { DropdownFilterComponent } from "../../../shared/components/dropdown-filter/dropdown-filter.component";

@Component({
  selector: 'app-requests',
  imports: [
    CommonModule,
    MaterialModule,
    ListContainerComponent,
    ListFilterBarComponent,
    ListRowComponent,
    DropdownFilterComponent
],
  templateUrl: './requests.component.html'
})
export class RequestsComponent {
  role = signal<UserRole>('Doctor'); 

  search = signal('');
  selectedRange = signal<FutureTimeRange>('All');
  selectedSlot = signal<'All' | TimeSlot>('All');

  private _appointments = signal<AppointmentResponseDto[]>([
    {
      id: '1',
      status: 'Requested',
      date: '2026-02-10',
      timeSlot: 'Morning',
      patient: { id: 'p1', firstName: 'Rahul', lastName: 'Sharma' },
      doctor: { id: 'd1', firstName: 'Amit', lastName: 'Singh' }
    },
    {
      id: '2',
      status: 'Requested',
      date: '2026-02-11',
      timeSlot: 'Evening',
      patient: { id: 'p2', firstName: 'Anita', lastName: 'Verma' },
      doctor: { id: 'd1', firstName: 'Amit', lastName: 'Singh' }
    }
  ]);
  public get appointments() {
    return this._appointments;
  }
  public set appointments(value) {
    this._appointments = value;
  }

  columns = computed(() => [
    this.role() === 'Doctor' ? 'Patient' : 'Doctor',
    'Date',
    'Slot'
  ]);

  filteredAppointments = computed(() => {
    const today = new Date();

    return this.appointments().filter(a => {

      if (a.status !== 'Requested') return false;

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

  getCounterpartyName(a: AppointmentResponseDto): string {
    return this.role() === 'Doctor'
      ? `${a.patient.firstName} ${a.patient.lastName}`
      : `${a.doctor.firstName} ${a.doctor.lastName}`;
  }

  getSlotLabel(slot: TimeSlot) {
    return TIME_SLOT_LABEL[slot];
  }

  onRangeChange(value: string) {
    this.selectedRange.set(value as FutureTimeRange);
  }

  onSlotChange(value: string) {
    this.selectedSlot.set(value as 'All' | TimeSlot);
  }

  onInfo(id: string) {
    console.log('Requested appointment:', id);
  }
}
