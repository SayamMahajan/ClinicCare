using ClinicCare.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClinicCare.Shared.DTOs.Patient
{
    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } 
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public DateOnly DOB { get; set; }
        public Gender Gender { get; set; }
        public string? EmergencyContact { get; set; }
        public string? BloodGroup { get; set; }
        public string? Allergies { get; set; }
        public decimal? BodyWeight { get; set; }
        public decimal? Height { get; set; }
        public string? Address { get; set; }
    }
}
