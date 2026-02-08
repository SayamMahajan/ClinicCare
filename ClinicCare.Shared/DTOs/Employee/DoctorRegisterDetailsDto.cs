using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Employee
{
    public class DoctorRegisterDetailsDto
    {
        public Guid SpecializationId { get; set; }

        [Precision(10, 2)]
        public decimal Fee { get; set; }

        public DateOnly FirstPracticeDate { get; set; }
    }
}
