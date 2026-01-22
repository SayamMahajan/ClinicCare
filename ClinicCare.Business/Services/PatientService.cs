using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.Business.Utils;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Auth;
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

        public async Task<IEnumerable<PatientResponseDto>> GetAllAsync()
        {
            var patients = await _repo.GetAllAsync();

            return patients.Select(p => new PatientResponseDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Phone = p.Phone
            });
        }

        public async Task<PatientResponseDto?> GetByIdAsync(Guid id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            return new PatientResponseDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email,
                Phone = patient.Phone
            };
        }

        public async Task UpdateAsync(Guid id, PatientUpdateDto dto)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Phone = dto.Phone;
            patient.Address = dto.Address;
            patient.Password = dto.Password;

            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var patient = await _repo.GetByIdAsync(id);
            if (patient is null)
                throw new NotFoundException($"Patient with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != id)
                throw new ForbiddenException("You are not authorized");

            await _repo.DeleteAsync(id);
            await _repo.SaveChangesAsync();
        }

        public async Task<Guid> RegisterPatientAsync(PatientRegisterDto dto)
        {
            var exists = await _repo
                .FindAsync(p => p.Email == dto.Email);

            if (exists.Any())
                throw new ConflictException("Email already registered");

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

            await _repo.InsertAsync(patient);
            await _repo.SaveChangesAsync();

            return patient.Id;
        }

        public async Task<PatientLoginResponseDto> LoginPatientAsync(PatientLoginDto dto)
        {
            var patients = await _repo.FindAsync(p => dto.Email.Trim().ToLower() == p.Email);
            var patient = patients.FirstOrDefault();

            if (patient is null ||
                !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, patient.Password))
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
    }
}