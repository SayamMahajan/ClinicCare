using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Helpers
{
    internal class TimeSlotHelper
    {
        public static TimeOnly GetStartTime(TimeSlotType slot) => slot switch
        {
            TimeSlotType.Morning => new TimeOnly(9, 0),
            TimeSlotType.Earlynoon => new TimeOnly(12, 0),
            TimeSlotType.Latenoon => new TimeOnly(15, 0),
            TimeSlotType.Evening => new TimeOnly(18, 0),
            TimeSlotType.Night => new TimeOnly(21, 0),
            _ => throw new ArgumentException("Invalid time slot")
        };

        public static TimeOnly GetEndTime(TimeSlotType slot) => slot switch
        {
            TimeSlotType.Morning => new TimeOnly(12, 0),
            TimeSlotType.Earlynoon => new TimeOnly(15, 0),
            TimeSlotType.Latenoon => new TimeOnly(18, 0),
            TimeSlotType.Evening => new TimeOnly(21, 0),
            TimeSlotType.Night => new TimeOnly(2, 0),   
            _ => throw new ArgumentException("Invalid time slot")
        };

        public static DateTime GetEndDateTime(TimeSlotType slot, DateOnly date)
        {
            var endTime = GetEndTime(slot);
            if (slot == TimeSlotType.Night)
                return date.ToDateTime(endTime).AddDays(1);
            return date.ToDateTime(endTime);
        }
    }
}
