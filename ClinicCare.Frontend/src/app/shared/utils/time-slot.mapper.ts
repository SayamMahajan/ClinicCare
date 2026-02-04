import { TimeSlot } from '../models/appointment.model'

export const TIME_SLOT_LABEL: Record<TimeSlot, string> = {
  Morning: '09:00AM - 12:00PM (Morning)',
  Earlynoon: '12:00PM - 3:00PM (Early Noon)',
  Latenoon: '3:00PM - 6:00PM (Late Noon)',
  Evening: '6:00PM - 9:00PM (Evening)',
  Night: '9:00PM - 2:00AM (Night)'
};
