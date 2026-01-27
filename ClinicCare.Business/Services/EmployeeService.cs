using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.Enums;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _repo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IGenericRepository<DoctorSpecialization> _specializationRepo;
        private readonly IJwtTokenGenerator _jwt;
        private readonly ICurrentUser _currentUser;

        public EmployeeService(
            IGenericRepository<Employee> repo,
            IEmployeeRepository employeeRepo,
            IGenericRepository<DoctorSpecialization> specializationRepo,
            IJwtTokenGenerator jwt,
            ICurrentUser currentUser)
        {
            _repo = repo;
            _employeeRepo = employeeRepo;
            _specializationRepo = specializationRepo;
            _jwt = jwt;
            _currentUser = currentUser;
        }

        public async Task<EmployeeLoginResponseDto> LoginAsync(EmployeeLoginDto dto)
        {
            ValidationHelper.NotNull(dto, "Login data is required.");

            dto.Email = NormalizationHelper.NormalizeKey(dto.Email);

            var employees = await _repo.FindAsync(e => dto.Email == e.Email);
            var employee = employees.FirstOrDefault();

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
            ValidationHelper.NotNull(dto, "Employee data is required.");

            dto.Email = NormalizationHelper.NormalizeKey(dto.Email);
            dto.FirstName = NormalizationHelper.NormalizeKey(dto.FirstName);
            dto.LastName = NormalizationHelper.NormalizeKey(dto.LastName);
            dto.Password = dto.Password.Trim();

            ValidationHelper.DateNotInFuture(dto.DateOfJoining, nameof(dto.DateOfJoining));

            var exists = await _repo
                .FindAsync(e => e.Email == dto.Email);

            if (exists.Any())
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
                DateOfJoining = dto.DateOfJoining,
                Password = hashedPassword
            };

            if (dto.Role == EmployeeRole.Doctor)
            {
                if (dto.DoctorDetails is null)
                    throw new BadRequestException("Doctor details required");

                if(_specializationRepo.GetByIdAsync(dto.DoctorDetails.SpecializationId) is null)
                    throw new BadRequestException($"Specialization with id {dto.DoctorDetails.SpecializationId} not found.");

                if (dto.DoctorDetails.Fee < 0)
                    throw new BadRequestException("Fee can't be negative.");

                ValidationHelper.DateNotInFuture(dto.DoctorDetails.FirstPracticeDate, nameof(dto.DoctorDetails.FirstPracticeDate));

                ValidationHelper.ValidateAge(dto.DoctorDetails.DOB);

                if (dto.DoctorDetails.FirstPracticeDate < dto.DoctorDetails.DOB.AddYears(18))
                    throw new BadRequestException("First practice date is unrealistically early.");

                employee.DoctorDetails = new DoctorDetail
                {
                    DoctorId = employee.Id,
                    SpecializationId = dto.DoctorDetails.SpecializationId,
                    Fee = dto.DoctorDetails.Fee,
                    DOB = dto.DoctorDetails.DOB,
                    Phone = dto.DoctorDetails.Phone.Trim(),
                    FirstPracticeDate = dto.DoctorDetails.FirstPracticeDate
                };
            }

            await _repo.InsertAsync(employee);
            await _repo.SaveChangesAsync();

            return employee.Id;
        }

        public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync(EmployeeRole? role)
        {
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
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var employee = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException("Employee not found");

            if (_currentUser.Role != UserRole.Admin && (employee.Role == EmployeeRole.Admin))
                throw new ForbiddenException("You are not authorized");

            return MapToDto(employee);
        }

        public async Task UpdateAsync(Guid id, EmployeeUpdateDto dto)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            ValidationHelper.NotNull(dto, "Employee data is required.");

            var employee = await _repo.GetByIdAsync(id)
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
                    if (_specializationRepo.GetByIdAsync(dto.SpecializationId.Value) is null)
                        throw new BadRequestException($"Specialization with id {dto.SpecializationId} not found.");
                    employee.DoctorDetails.SpecializationId = dto.SpecializationId!.Value;

                if (!string.IsNullOrWhiteSpace(dto.Phone))
                    employee.DoctorDetails.Phone = dto.Phone.Trim();
            }

            await _repo.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

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