using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Auth;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _repo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IJwtTokenGenerator _jwt;
        private readonly ICurrentUser _currentUser;

        public EmployeeService(
            IGenericRepository<Employee> repo,
            IEmployeeRepository employeeRepo,
            IJwtTokenGenerator jwt,
            ICurrentUser currentUser)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
            _jwt = jwt;
            _currentUser = currentUser;
        }

        public async Task<EmployeeLoginResponseDto> LoginAsync(LoginRequestDto dto)
        {
            var employees = await _repo.FindAsync(e => dto.Email.Trim().ToLower() == e.Email);
            var employee = employees.FirstOrDefault();

            if (employee is null ||
                !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, employee.Password))
                throw new UnauthorizedException("Invalid email or password");

            return new EmployeeLoginResponseDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = employee.Role,
                Token = _jwt.GenerateEmployeeToken(employee)
            };
        }

        public async Task<Guid> RegisterAsync(EmployeeRegisterDto dto)
        {
            var exists = await _repo
                .FindAsync(e => e.Email == dto.Email);

            if (exists.Any())
                throw new ConflictException("Email already registered");

            var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                DateOfJoining = dto.DateOfJoining,
                Password = hashedPassword
            };

            if (dto.Role == EmployeeRole.Doctor)
            {
                if (dto.DoctorDetails is null)
                    throw new ValidationException("Doctor details required");

                employee.DoctorDetails = new DoctorDetail
                {
                    DoctorId = employee.Id,
                    SpecializationId = dto.DoctorDetails.SpecializationId,
                    Fee = dto.DoctorDetails.Fee,
                    DOB = dto.DoctorDetails.DOB,
                    Phone = dto.DoctorDetails.Phone,
                    FirstPracticeDate = dto.DoctorDetails.FirstPracticeDate
                };
            }

            await _repo.InsertAsync(employee);
            await _repo.SaveChangesAsync();

            return employee.Id;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync(EmployeeRole? role)
        {
            if(_currentUser.Role != UserRole.Admin && (role == EmployeeRole.Admin || role is null))
                throw new ForbiddenException("You are not authorized");

            IEnumerable<Employee> employees;

            if (role is null)
                employees = await _repo.GetAllAsync();
            else if (role == EmployeeRole.Doctor)
                employees = await _employeeRepo.GetDoctorsAsync(null);
            else 
                employees = await _repo.FindAsync(e => e.Role == role.Value);

            return employees.Select(MapToDto);
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllDoctorsAsync(Guid? specializationId)
        {
            IEnumerable<Employee> employees = await _employeeRepo.GetDoctorsAsync(specializationId);
            return employees.Select(MapToDto);
        }

        public async Task<EmployeeResponseDto> GetByIdAsync(Guid id)
        {
            var employee = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            if (_currentUser.Role != UserRole.Admin && (employee.Role == EmployeeRole.Admin))
                throw new ForbiddenException("You are not authorized");

            return MapToDto(employee);
        }

        public async Task UpdateAsync(Guid id, EmployeeUpdateDto dto)
        {
            var employee = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            employee.FirstName = dto.FirstName;
            employee.LastName = dto.LastName;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                employee.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(dto.Password);
            
            if (employee.Role == EmployeeRole.Doctor)
            {
                if (employee.DoctorDetails is null)
                    throw new DataException("Doctor details missing");

                if (dto.Fee.HasValue)
                    employee.DoctorDetails.Fee = dto.Fee.Value;

                if (dto.SpecializationId.HasValue)
                    employee.DoctorDetails.SpecializationId = dto.SpecializationId.Value;

                if (!string.IsNullOrWhiteSpace(dto.Phone))
                    employee.DoctorDetails.Phone = dto.Phone;
            }

            await _repo.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            var employee = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        private static EmployeeResponseDto MapToDto(Employee e)
        {
            return new EmployeeResponseDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Role = e.Role,
                DateOfJoining = e.DateOfJoining,

                SpecializationId = e.DoctorDetails?.SpecializationId,
                Fee = e.DoctorDetails?.Fee,
                Phone = e.DoctorDetails?.Phone,
                DOB = e.DoctorDetails?.DOB,
                FirstPracticeDate = e.DoctorDetails?.FirstPracticeDate
            };
        }
    }
}