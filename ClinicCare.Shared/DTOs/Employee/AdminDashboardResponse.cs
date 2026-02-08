namespace ClinicCare.Shared.DTOs.Employee
{
    public class AdminDashboardResponse
    {
        public int AppointmentsToday { get; set; }
        public int AppointmentsThisMonth { get; set; }
        public int TotalDoctors { get; set; }
        public int NewPatientsToday { get; set; } 
        public int NewPatientsThisMonth { get; set; }
    }
}
