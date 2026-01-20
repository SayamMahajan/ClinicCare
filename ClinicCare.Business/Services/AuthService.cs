using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Auth;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<Patient> _patientRepo;
        private readonly IGenericRepository<Employee> _employeeRepo;
        private readonly IGenericRepository<DoctorDetail> _doctorDetailRepo;
        private readonly IJwtTokenGenerator _jwt;

        public AuthService(IGenericRepository<Patient> patientRepo, 
            IGenericRepository<Employee> employeeRepo,
            IGenericRepository<DoctorDetail> doctorDetailRepo,
            IJwtTokenGenerator jwt)
        {
            _patientRepo = patientRepo;
            _employeeRepo = employeeRepo;
            _doctorDetailRepo = doctorDetailRepo;
            _jwt = jwt;
        }

        public async Task<PatientAuthResponseDto> LoginPatientAsync(LoginRequestDto dto)
        {
            var patients = await _patientRepo.FindAsync(p => dto.Email.Trim().ToLower() == p.Email);
            var patient = patients.FirstOrDefault();
            if (patient is null)
                throw new UnauthorizedException("Invalid email or password.");

            var isPatientVerified = BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, patient.Password);

            if (!isPatientVerified)
                throw new UnauthorizedException("Invalid email or password.");

            var token = _jwt.GeneratePatientToken(patient);

            return new PatientAuthResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Token = token
            };
        }

        public async Task<EmployeeAuthResponseDto> LoginEmployeeAsync(LoginRequestDto dto)
        {
            var employees = await _employeeRepo.FindAsync(e => dto.Email.Trim().ToLower() == e.Email);
            var employee = employees.FirstOrDefault();

            if (employee is null)
                throw new UnauthorizedException("Invalid email or password.");

            var isEmployeeVerified = BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, employee.Password);

            if (!isEmployeeVerified)
                throw new UnauthorizedException("Invalid email or password.");

            var token = _jwt.GenerateEmployeeToken(employee);

            return new EmployeeAuthResponseDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = employee.Role,
                Token = token
            };
        }

        public async Task<Guid> RegisterPatientAsync(PatientRegisterDto dto)
        {
            var exists = await _patientRepo
                .FindAsync(p => p.Email == dto.Email);

            if (exists.Any())
                throw new BadRequestException("Email already registered");

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);

            var patient = new Patient
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                DOB = dto.DOB,
                Gender = dto.Gender,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = hashedPassword
            };

            await _patientRepo.InsertAsync(patient);
            await _patientRepo.SaveChangesAsync();

            return patient.Id;
        }

        public async Task<Guid> RegisterAdminAsync(AdminRegisterDto dto)
        {
            var exists = await _employeeRepo
                .FindAsync(p => p.Email == dto.Email);

            if (exists.Any())
                throw new BadRequestException("Email already registered");

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);

            var admin = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = EmployeeRole.Admin,
                Email = dto.Email,
                DateOfJoining = dto.DateOfJoining,
                Password = hashedPassword
            };

            await _employeeRepo.InsertAsync(admin);
            await _employeeRepo.SaveChangesAsync();

            return admin.Id;
        }

        public async Task<Guid> RegisterDoctorAsync(DoctorRegisterDto dto)
        {
            var exists = await _employeeRepo
                .FindAsync(p => p.Email == dto.Email);

            if (exists.Any())
                throw new BadRequestException("Email already registered");

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);

            var doctor = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Role = EmployeeRole.Doctor,
                Email = dto.Email,
                DateOfJoining = dto.DateOfJoining,
                Password = hashedPassword
            };

            await _employeeRepo.InsertAsync(doctor);
            await _employeeRepo.SaveChangesAsync();

            var doctorDetails = new DoctorDetail
            {
                DoctorId = doctor.Id,
                SpecializationId = dto.SpecializationId,
                Fee = dto.Fee,
                DOB = dto.DOB,
                Phone = dto.Phone,
                FirstPracticeDate = dto.FirstPracticeDate,
            };

            await _doctorDetailRepo.InsertAsync(doctorDetails);
            await _doctorDetailRepo.SaveChangesAsync();

            return doctor.Id;
        }
    }
}