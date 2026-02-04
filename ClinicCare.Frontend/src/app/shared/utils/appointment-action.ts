import { AppointmentResponseDto } from '../models/appointment.model';

export function resolveAppointmentActions(
  appointment: AppointmentResponseDto,
  section: 'requested' | 'scheduled' | 'history'
) {
  const today = new Date();
  const aptDate = new Date(appointment.date);
  const hasPrescription = !!appointment.prescriptionId;

  if (section === 'requested') {
    return [{ type: 'delete', label: 'Delete', color: 'warn' }];
  }

  if (section === 'scheduled') {
    if (aptDate > today && !hasPrescription) {
      return [{ type: 'delete', label: 'Delete', color: 'warn' }];
    }

    if (aptDate <= today && !hasPrescription) {
      return [{ type: 'addPrescription', label: 'Add Prescription' }];
    }

    if (hasPrescription) {
      return [{ type: 'viewPrescription', label: 'View Prescription' }];
    }
  }

  if (section === 'history') {
    return [{ type: 'viewPrescription', label: 'View Prescription' }];
  }

  return [];
}
