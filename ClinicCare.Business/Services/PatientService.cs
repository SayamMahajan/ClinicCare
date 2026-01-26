using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;

namespace ClinicCare.Business.Services
{
    public class PatientService : IPatientService
    {
        private readonly IGenericRepository<Patient> _repo;
        private readonly IJwtTokenGenerator _jwt;
        private readonly ICurrentUser _currentUser;

        public PatientService(
            IGenericRepository<Patient> repo,
            IJwtTokenGenerator jwt,
            ICurrentUser currentUser
            )
        {
            _repo = repo;
            _jwt = jwt;
            _currentUser = currentUser;
            
        }

        public async Task<PatientLoginResponseDto> LoginPatientAsync(PatientLoginDto dto)
        {
            ValidationHelper.NotNull(dto, "Login data is required.");

            dto.Email = NormalizationHelper.NormalizeKey(dto.Email);

            var patients = await _repo.FindAsync(p => dto.Email == p.Email);
            var patient = patients.FirstOrDefault();

            if (patient is null || !PasswordHelper.Verify(dto.Password, patient.Password))
                throw new UnauthorizedException("Invalid email or password.");

            var token = _jwt.GeneratePatientToken(patient);

            return new PatientLoginResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Token = token
            };
        }

        public async Task<Guid> RegisterPatientAsync(PatientRegisterDto dto)
        {
            ValidationHelper.NotNull(dto, "Patient data is required.");

            dto.Email = NormalizationHelper.NormalizeKey(dto.Email);
            dto.FirstName = NormalizationHelper.NormalizeKey(dto.FirstName);
            dto.LastName = NormalizationHelper.NormalizeKey(dto.LastName);
            dto.Phone = dto.Phone.Trim();
            dto.Password = dto.Password.Trim();

            var exists = await _repo
                .FindAsync(p => p.Email == dto.Email);

            if (exists.Any())
                throw new ConflictException("Email already registered");

            PasswordHelper.Validate(dto.Password);
            var hashedPassword = PasswordHelper.Hash(dto.Password);

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

            await _repo.InsertAsync(patient);
            await _repo.SaveChangesAsync();

            return patient.Id;
        }

        public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();

            return patients.Select(MapToDto);
        }

        public async Task<PatientResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            return MapToDto(patient);
        }

        public async Task UpdateAsync(Guid id, PatientUpdateDto dto)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));
            ValidationHelper.NotNull(dto, "Patient data is required.");

            var patient = await _repo.GetByIdAsync(id)
                ?? throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized.");

            if (dto.FirstName is not null)
                patient.FirstName =
                    NormalizationHelper.NormalizeKey(dto.FirstName);

            if (dto.LastName is not null)
                patient.LastName =
                    NormalizationHelper.NormalizeKey(dto.LastName);

            if (dto.Phone is not null)
                patient.Phone = dto.Phone.Trim();

            if (dto.Address is not null)
                patient.Address = dto.Address.Trim();

            if (dto.Password is not null)
            {
                dto.Password = dto.Password.Trim();
                PasswordHelper.Validate(dto.Password);
                patient.Password = PasswordHelper.Hash(dto.Password);
            }

            if (dto.EmergencyContact is not null)
                patient.EmergencyContact = dto.EmergencyContact.Trim();

            if (dto.BloodGroup is not null)
                patient.BloodGroup = NormalizationHelper.NormalizeKey(dto.BloodGroup);

            if (dto.Allergies is not null)
                patient.Allergies = NormalizationHelper.NormalizeKey(dto.Allergies);

            if (dto.BodyWeight is not null)
                patient.BodyWeight = dto.BodyWeight;

            if (dto.Height is not null)
                patient.Height = dto.Height;

            if (dto.Address is not null)
                patient.Address = NormalizationHelper.NormalizeKey(dto.Address);

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }
        private static PatientResponseDto MapToDto(Patient p)
        {
            return new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Phone = p.Phone
            };
        }
    }
}