using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Doctor;
using ClinicCare.Shared.DTOs.Enums;
using System.Numerics;

namespace ClinicCare.Business.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IGenericRepository<DoctorDetails> _doctorRepo;
        private readonly IGenericRepository<Employee> _employeeRepo;

        public DoctorService(IGenericRepository<DoctorDetails> doctorRepo, IGenericRepository<Employee> employeeRepo)
        {
            _doctorRepo = doctorRepo;
            _employeeRepo = employeeRepo;
        }

        public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync()
        {
            var doctorDetailsList = await _doctorRepo.GetAllAsync();
            var doctorResponses = new List<DoctorResponseDto>();

            foreach (var doctorDetail in doctorDetailsList)
            {
                var employee = await _employeeRepo.GetByIdAsync(doctorDetail.DoctorId);

                if (employee is not null && employee.Role == EmployeeRole.Doctor)
                {
                    doctorResponses.Add(new DoctorResponseDto
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        Role = employee.Role,
                        DateOfJoining = employee.DateOfJoining,
                        Fee = doctorDetail.Fee,
                        SpecialistType = doctorDetail.SpecialistType,
                        DOB = doctorDetail.DOB,
                        Phone = doctorDetail.Phone,
                        FirstPracticeDate = doctorDetail.FirstPracticeDate
                    });
                }
            }

            return doctorResponses;
        }

        public async Task<DoctorResponseDto?> GetByIdAsync(int id)
        {
            var doctor = await _employeeRepo.GetByIdAsync(id);
            var doctorDetails = await _doctorRepo.GetByIdAsync(id);

            if (doctor is null || doctorDetails is null || doctor.Role != EmployeeRole.Doctor) return null;

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                Role = doctor.Role,
                DateOfJoining = doctor.DateOfJoining,
                Fee = doctorDetails.Fee,
                SpecialistType = doctorDetails.SpecialistType,
                DOB = doctorDetails.DOB,
                Phone = doctorDetails.Phone,
                FirstPracticeDate = doctorDetails.FirstPracticeDate,
            };
        }

        public async Task<IEnumerable<DoctorResponseDto>> GetBySpecialistTypeAsync(string type)
        {
            var doctorDetailsList = await _doctorRepo.FindAsync(d => d.SpecialistType == type);
            var doctorResponses = new List<DoctorResponseDto>();

            foreach (var doctorDetail in doctorDetailsList)
            {
                var employee = await _employeeRepo.GetByIdAsync(doctorDetail.DoctorId);

                if (employee is not null && employee.Role == EmployeeRole.Doctor)
                {
                    doctorResponses.Add(new DoctorResponseDto
                    {
                        Id = employee.Id,
                        FirstName = employee.FirstName,
                        LastName = employee.LastName,
                        Email = employee.Email,
                        Role = employee.Role,
                        DateOfJoining = employee.DateOfJoining,
                        Fee = doctorDetail.Fee,
                        SpecialistType = doctorDetail.SpecialistType,
                        DOB = doctorDetail.DOB,
                        Phone = doctorDetail.Phone,
                        FirstPracticeDate = doctorDetail.FirstPracticeDate
                    });
                }
            }

            return doctorResponses;
        }

        public async Task UpdateAsync(int id, DoctorUpdateDto dto)
        {
            var doctor = await _employeeRepo.GetByIdAsync(id);
            var doctorDetails = await _doctorRepo.GetByIdAsync(id);

            if (doctor is null || doctorDetails is null || doctor.Role != EmployeeRole.Doctor) return;

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Password = dto.Password;

            doctorDetails.Fee = dto.Fee;
            doctorDetails.SpecialistType = dto.SpecialistType;
            doctorDetails.Phone = dto.Phone;

            await _employeeRepo.SaveChangesAsync();
            await _doctorRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var doctor = await _employeeRepo.GetByIdAsync(id);
            if (doctor is null || doctor.Role != EmployeeRole.Admin) return;

            await _employeeRepo.Delete(id);
            await _employeeRepo.SaveChangesAsync();
        }
    }
}