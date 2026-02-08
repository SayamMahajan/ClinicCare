using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.Enums;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepo;
        private readonly ISpecializationRepository _specializationRepo;
        private readonly IPatientRepository _patientRepo;
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IJwtTokenGenerator _jwt;
        private readonly ICurrentUser _currentUser;

        public EmployeeService(
            IEmployeeRepository employeeRepo,
            ISpecializationRepository specializationRepo,
            IPatientRepository patientRepo,
            IAppointmentRepository appointmentRepo,
            IJwtTokenGenerator jwt,
            ICurrentUser currentUser)
        {
            _employeeRepo = employeeRepo;
            _specializationRepo = specializationRepo;
            _patientRepo = patientRepo;
            _appointmentRepo = appointmentRepo;
            _jwt = jwt;
            _currentUser = currentUser;
        }

        public async Task<EmployeeLoginResponseDto> LoginAsync(EmployeeLoginDto dto)
        {
            ValidationHelper.NotNull(dto, "Login data is required.");

            dto.Email = NormalizationHelper.NormalizeKey(dto.Email);

            var employee = await _employeeRepo.GetByEmailAsync(dto.Email);

            if (employee is null || !PasswordHelper.Verify(dto.Password, employee.Password))
                throw new UnauthorizedException("Invalid email or password");

            var token = _jwt.GenerateEmployeeToken(employee);

            return new EmployeeLoginResponseDto
            {
                Id = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Role = employee.Role,
                Token = token,
            };
        }

        public async Task<Guid> RegisterAsync(EmployeeRegisterDto dto)
        {
            ValidationHelper.ValidateAge(dto.DOB);

            if (dto.Role is EmployeeRole.Admin)
                throw new BadRequestException("Cannot register admin");

            ValidationHelper.NotNull(dto, "Employee data is required.");

            dto.Email = NormalizationHelper.NormalizeKey(dto.Email);
            dto.FirstName = NormalizationHelper.NormalizeKey(dto.FirstName);
            dto.LastName = NormalizationHelper.NormalizeKey(dto.LastName);
            dto.Password = dto.Password.Trim();

            var employeeExists = await _employeeRepo.GetByEmailAsync(dto.Email);

            if (employeeExists is not null)
                throw new ConflictException("Email already registered");

            PasswordHelper.Validate(dto.Password);
            var hashedPassword = PasswordHelper.Hash(dto.Password);

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Role = dto.Role,
                Password = hashedPassword,
                Gender = dto.Gender,
                DOB = dto.DOB,
                Phone = dto.Phone.Trim(),
            };

            if (dto.Role == EmployeeRole.Doctor)
            {
                if (dto.DoctorDetails is null)
                    throw new BadRequestException("Doctor details required");

                var spec = await _specializationRepo.GetByIdAsync(dto.DoctorDetails.SpecializationId);
                if (spec is null)
                    throw new BadRequestException($"Specialization with id {dto.DoctorDetails.SpecializationId} not found.");

                if (dto.DoctorDetails.Fee < 0)
                    throw new BadRequestException("Fee can't be negative.");

                ValidationHelper.DateNotInFuture(dto.DoctorDetails.FirstPracticeDate, nameof(dto.DoctorDetails.FirstPracticeDate));

                if (dto.DoctorDetails.FirstPracticeDate < dto.DOB.AddYears(18))
                    throw new BadRequestException("First practice date is unrealistically early.");

                employee.DoctorDetails = new DoctorDetail
                {
                    DoctorId = employee.Id,
                    SpecializationId = dto.DoctorDetails.SpecializationId,
                    Fee = dto.DoctorDetails.Fee,
                    FirstPracticeDate = dto.DoctorDetails.FirstPracticeDate
                };
            }

            await _employeeRepo.InsertAsync(employee);
            await _employeeRepo.SaveChangesAsync();

            return employee.Id;
        }

        public async Task<PaginatedResult<EmployeeResponseDto>> GetAllAsync(EmployeeSearchParams searchParams)
        {
            if(_currentUser.Role != UserRole.Admin)
                throw new ForbiddenException("You are not authorized");

            var result = await _employeeRepo.GetAllAsync(searchParams);
            return MapPaginatedResult(result);
        }

        public async Task<AdminDashboardResponse> GetAdminDashboardAsync()
        {
            if (_currentUser.Role != UserRole.Admin)
                throw new ForbiddenException("Admin access only");

            var today = DateOnly.FromDateTime(DateTime.Today);
            var monthStart = today.AddDays(1 - today.Day);

            var apptTodayTask = _appointmentRepo.GetTodayCountAsync();
            var apptMonthTask = _appointmentRepo.GetThisMonthCountAsync(monthStart);
            var doctorsTask = _employeeRepo.GetTotalDoctorsCountAsync();
            var patientsTodayTask = _patientRepo.GetTodayCountAsync();
            var patientsMonthTask = _patientRepo.GetThisMonthCountAsync(monthStart);

            await Task.WhenAll(apptTodayTask, apptMonthTask, doctorsTask, patientsTodayTask, patientsMonthTask);

            return new AdminDashboardResponse
            {
                AppointmentsToday = await apptTodayTask,
                AppointmentsThisMonth = await apptMonthTask, 
                TotalDoctors = await doctorsTask,
                NewPatientsToday = await patientsTodayTask,
                NewPatientsThisMonth = await patientsMonthTask,
            };
        }


        public async Task<EmployeeResponseDto> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var employee = await _employeeRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            if (_currentUser.Role != UserRole.Admin && employee.Role == EmployeeRole.Admin)
                throw new ForbiddenException("You are not authorized");

            if (employee.Role == EmployeeRole.Doctor)
                employee = await _employeeRepo.GetDoctorByIdAsync(id);

            return MapToDto(employee!);
        }

        public async Task UpdateAsync(Guid id, EmployeeUpdateDto dto)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            ValidationHelper.NotNull(dto, "Employee data is required.");

            var employee = await _employeeRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            if (dto.FirstName is not null)
                employee.FirstName =
                    NormalizationHelper.NormalizeKey(dto.FirstName);

            if (dto.LastName is not null)
                employee.LastName =
                    NormalizationHelper.NormalizeKey(dto.LastName);

            if (dto.Password is not null)
            {
                dto.Password = dto.Password.Trim();
                PasswordHelper.Validate(dto.Password);
                employee.Password = PasswordHelper.Hash(dto.Password);

            }

            if (employee.Role == EmployeeRole.Doctor && employee.DoctorDetails is not null)
            {
                if (dto.Fee is not null)
                    if (dto.Fee.Value <= 0)
                        throw new BadRequestException("Fee must be greater than 0.");
                    employee.DoctorDetails.Fee = dto.Fee!.Value;

                if (dto.SpecializationId.HasValue)
                {
                    var spec = await _specializationRepo.GetByIdAsync(dto.SpecializationId.Value);
                    if (spec is null)
                        throw new BadRequestException($"Specialization with id {dto.SpecializationId} not found.");
                    employee.DoctorDetails.SpecializationId = dto.SpecializationId.Value;
                }

                if (!string.IsNullOrWhiteSpace(dto.Phone))
                    employee.Phone = dto.Phone.Trim();
            }

            await _employeeRepo.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var employee = await _employeeRepo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            await _employeeRepo.DeleteAsync(id);
            await _employeeRepo.SaveChangesAsync();
        }

        private PaginatedResult<EmployeeResponseDto> MapPaginatedResult(PaginatedResult<Employee> result)
        {
            return new PaginatedResult<EmployeeResponseDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
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
                Gender = e.Gender,
                Phone = e.Phone,
                DOB = e.DOB,
                CreatedAt = e.CreatedAt,

                SpecializationId = e.DoctorDetails?.SpecializationId,
                Fee = e.DoctorDetails?.Fee,
                FirstPracticeDate = e.DoctorDetails?.FirstPracticeDate
            };
        }
    }
}