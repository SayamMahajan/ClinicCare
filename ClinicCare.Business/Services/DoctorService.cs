using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Doctor;
using ClinicCare.Shared.Enums;
using System.Numerics;

namespace ClinicCare.Business.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IGenericRepository<DoctorDetail> _doctorRepo;
        private readonly IGenericRepository<Employee> _employeeRepo;
        private readonly ICurrentUser _currentUser;

        public DoctorService(
            IGenericRepository<DoctorDetail> doctorRepo,
            IGenericRepository<Employee> employeeRepo, 
            ICurrentUser currentUser
            )
        {
            _doctorRepo = doctorRepo;
            _employeeRepo = employeeRepo;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<DoctorResponseDto>> GetAllAsync(Guid? specializationId = null)
        {
            var doctorDetailsList = specializationId is null
                ? await _doctorRepo.GetAllAsync()
                : await _doctorRepo.FindAsync(d => d.SpecializationId == specializationId);

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
                        SpecializationId = doctorDetail.SpecializationId,
                        DOB = doctorDetail.DOB,
                        Phone = doctorDetail.Phone,
                        FirstPracticeDate = doctorDetail.FirstPracticeDate
                    });
                }
            }

            return doctorResponses;
        }

        public async Task<DoctorResponseDto?> GetByIdAsync(Guid id)
        {
            var doctor = await _employeeRepo.GetByIdAsync(id);
            var doctorDetails = await _doctorRepo.GetByIdAsync(id);

            if (doctor is null || doctorDetails is null || doctor.Role != EmployeeRole.Doctor)
                throw new NotFoundException($"Doctor with id {id} not found.");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            return new DoctorResponseDto
            {
                Id = doctor.Id,
                FirstName = doctor.FirstName,
                LastName = doctor.LastName,
                Email = doctor.Email,
                Role = doctor.Role,
                DateOfJoining = doctor.DateOfJoining,
                Fee = doctorDetails.Fee,
                SpecializationId = doctorDetails.SpecializationId,
                DOB = doctorDetails.DOB,
                Phone = doctorDetails.Phone,
                FirstPracticeDate = doctorDetails.FirstPracticeDate,
            };
        }

        public async Task UpdateAsync(Guid id, DoctorUpdateDto dto)
        {
            var doctor = await _employeeRepo.GetByIdAsync(id);
            var doctorDetails = await _doctorRepo.GetByIdAsync(id);

            if (doctor is null || doctorDetails is null || doctor.Role != EmployeeRole.Doctor)
                throw new NotFoundException($"Doctor with id {id} not found.");

            doctor.FirstName = dto.FirstName;
            doctor.LastName = dto.LastName;
            doctor.Password = dto.Password;

            doctorDetails.Fee = dto.Fee;
            doctorDetails.SpecializationId = dto.SpecializationId;
            doctorDetails.Phone = dto.Phone;

            await _employeeRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var doctor = await _employeeRepo.GetByIdAsync(id);

            if (doctor is null || doctor.Role != EmployeeRole.Doctor)
                throw new NotFoundException($"Doctor with id {id} not found.");

            await _employeeRepo.Delete(id);
            await _employeeRepo.SaveChangesAsync();
        }
    }
}