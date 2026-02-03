import { Component, computed, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MaterialModule } from '../../../shared/ui/material.module';
import { ListFilterBarComponent } from '../../../shared/components/list-filter-bar/list-filter-bar.component';
import { ListContainerComponent } from '../../../shared/components/list-container/list-container.component';
import { ListRowComponent } from '../../../shared/components/list-row/list-row.component';
import { TimeSlot, DoctorDto } from '../../../shared/models/appointment.model';
import { DropdownFilterComponent } from "../../../shared/components/dropdown-filter/dropdown-filter.component";

@Component({
  selector: 'app-book-appointment',
  standalone: true,
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
export class BookAppointmentComponent {

  patientId = 'patient-123'; 

  search = signal('');
  selectedSpecialization = signal<'All' | string>('All');

  selectedDoctor = signal<DoctorDto | null>(null);
  selectedDate = signal<Date | null>(null);
  selectedSlot = signal<TimeSlot | null>(null);

  doctors = signal<DoctorDto[]>
  ([
    {
      id: 'd1',
      firstName: 'Amit',
      lastName: 'Singh',
      specialization: 'Cardiology',
      fee: 800,
      phone: '9876543210',
      firstPracticeDate: '2015-06-01'
    },
    {
      id: 'd2',
      firstName: 'Neha',
      lastName: 'Verma',
      specialization: 'Dermatology',
      fee: 600,
      phone: '9123456780',
      firstPracticeDate: '2018-03-15'
    },
  ]);

  specializations = computed(() => [
    ...new Set(this.doctors().map(d => d.specialization))
  ]);

  filteredDoctors = computed(() => {
    return this.doctors().filter(d => {
      const nameMatch =
        `${d.firstName} ${d.lastName}`
          .toLowerCase()
          .includes(this.search().toLowerCase());

      const specMatch =
        this.selectedSpecialization() === 'All' ||
        d.specialization === this.selectedSpecialization();

      return nameMatch && specMatch;
    });
  });

  submit() {
    if (!this.selectedDoctor() || !this.selectedDate() || !this.selectedSlot()) {
      return;
    }

    const payload = {
      patientId: this.patientId,
      doctorId: this.selectedDoctor()!.id,
      date: this.selectedDate(),
      timeSlot: this.selectedSlot()
    };

    console.log('AppointmentCreateDto', payload);
    // call API here
  }

  onSlotChange(value: string) {
    this.selectedSlot.set(value as null | TimeSlot);
  }

  selectDoctor(d: DoctorDto) {
    this.selectedDoctor.set(d);
  }

  getExperienceYears(firstPracticeDate?: string): string {
    if (!firstPracticeDate) return '-';

    const start = new Date(firstPracticeDate);
    const now = new Date();

    let years = now.getFullYear() - start.getFullYear();

    const m = now.getMonth() - start.getMonth();
    if (m < 0 || (m === 0 && now.getDate() < start.getDate())) {
      years--;
    }

    return `${years} yrs`;
  }

  isSelected(doc: DoctorDto): boolean {
    return this.selectedDoctor()?.id === doc.id;
  }

  toggleDoctor(doc: DoctorDto) {
    if (this.isSelected(doc)) {
      this.selectedDoctor.set(null);
    } else {
      this.selectedDoctor.set(doc);
    }
  }
}
