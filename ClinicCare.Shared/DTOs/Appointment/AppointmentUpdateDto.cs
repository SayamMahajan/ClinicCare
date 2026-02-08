using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Appointment
{
    public class AppointmentUpdateDto
    {
        [EnumDataType(typeof(AppointmentStatus))]
        public AppointmentStatus? Status { get; set; }
        public DateOnly? Date { get; set; }

        [EnumDataType(typeof(TimeSlotType))]
        public TimeSlotType? TimeSlot { get; set; }
    }
}
