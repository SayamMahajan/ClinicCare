using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Appointment;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.Enums;
using System.Data;
namespace ClinicCare.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepo;
        private readonly IEmployeeRepository _employeeRepo;
        private readonly IPaymentService _paymentService;
        private readonly ICurrentUser _currentUser;

        public AppointmentService(
            IAppointmentRepository appointmentRepo,
            IEmployeeRepository employeeRepo,
            IPaymentService paymentService,
            ICurrentUser currentUser
            )
        {
            _appointmentRepo = appointmentRepo;
            _employeeRepo = employeeRepo;
            _paymentService = paymentService;
            _currentUser = currentUser;
        }

        public async Task<PaginatedResult<AppointmentResponseDto>> GetAllAsync(AppointmentSearchParams searchParams)
        {
            PaginatedResult<Appointment> result = _currentUser.Role switch
            {
                UserRole.Admin => await _appointmentRepo.GetAllAsync(searchParams),
                UserRole.Doctor => await _appointmentRepo.GetAllAsync(searchParams, patientId: null, doctorId: _currentUser.UserId) ,
                UserRole.Patient => await _appointmentRepo.GetAllAsync(searchParams, patientId: _currentUser.UserId, doctorId: null),
                _ => throw new ForbiddenException("Invalid role")

            };

            return MapPaginatedResult(result);
        }

        public async Task<AppointmentResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with id {id} not found.");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != appointment.PatientId)
                throw new ForbiddenException($"You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != appointment.DoctorId)
                throw new ForbiddenException($"You are not authorized");

            return MapToDto(appointment);
        }

        public async Task<Guid> CreateAsync(AppointmentCreateDto dto)
        {
            ValidationHelper.GuidNotEmpty(dto.PatientId, nameof(dto.PatientId));
            ValidationHelper.GuidNotEmpty(dto.DoctorId, nameof(dto.DoctorId));
            ValidationHelper.GuidNotEmpty(dto.PaymentId, nameof(dto.PaymentId));

            if (_currentUser.Role == UserRole.Patient &&
                _currentUser.UserId != dto.PatientId)
                throw new ForbiddenException("You can book only for yourself.");

            ValidationHelper.DateAtLeast24HoursAdvance(dto.Date, nameof(dto.Date));

            var doctor = await _employeeRepo.GetDoctorByIdAsync(dto.DoctorId)
                ?? throw new NotFoundException($"Doctor with Id{dto.DoctorId} not found.");

            if (doctor.Role != EmployeeRole.Doctor || doctor.DoctorDetails is null)
                throw new BadRequestException("Invalid doctor.");

            var specializationId =
                doctor.DoctorDetails.SpecializationId;

            var patientAppointments = await _appointmentRepo.GetPatientAppointmentsForConflictCheckAsync(
                dto.PatientId, dto.Date, dto.TimeSlot);

            var conflict = patientAppointments.Any(a => a.Doctor.DoctorDetails!.SpecializationId == specializationId);


            if (conflict)
                throw new ConflictException(
                    "You already have an appointment with this specialization in this slot.");

            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Date = dto.Date,
                PaymentId = dto.PaymentId,
                TimeSlot = dto.TimeSlot,
                Status = AppointmentStatus.Requested
            };

            await _appointmentRepo.InsertAsync(appointment);
            await _appointmentRepo.SaveChangesAsync();

            return appointment.Id;
        }

        public async Task UpdateAsync(Guid id, AppointmentUpdateDto dto)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));
            ValidationHelper.NotNull(dto, "Appointment data is required.");

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment is null) 
                throw new NotFoundException($"Appointment with Id{id} not found.");

            if(_currentUser.Role != UserRole.Doctor)
                throw new ForbiddenException("You are not authorized");

            if(_currentUser.Role == UserRole.Doctor && appointment.DoctorId != _currentUser.UserId)
                throw new ForbiddenException("You are not authorized");

            if (appointment.Status == AppointmentStatus.Cancelled || appointment.Status == AppointmentStatus.Completed)
                throw new BadRequestException("Can't update this appointment");

            if (dto.Status == AppointmentStatus.Completed && appointment.Prescription is null)
                throw new BadRequestException("No prescription assigned yet.");

            if(dto.Status is not null)
            {
                if(dto.Status == AppointmentStatus.Cancelled)
                {
                    await _paymentService.ProcessCancellationRefundAsync(appointment);
                }
                appointment.Status = dto.Status.Value;
            }              

            if (dto.Date is not null)
            {
                ValidationHelper.DateNotInPast(dto.Date.Value, nameof(dto.Date));
                appointment.Date = dto.Date.Value;
            }

            if (dto.TimeSlot is not null)
                appointment.TimeSlot = dto.TimeSlot!.Value;

            await _appointmentRepo.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var appointment = await _appointmentRepo.GetByIdAsync(id);
            if (appointment is null)
                throw new NotFoundException($"Appointment with Id{id} not found.");

            if (_currentUser.Role == UserRole.Patient && appointment.PatientId != _currentUser.UserId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && appointment.DoctorId != _currentUser.UserId)
                throw new ForbiddenException("You are not authorized");

            if (appointment.Status == AppointmentStatus.Approved || appointment.Status == AppointmentStatus.Requested)
            {
                await _paymentService.ProcessCancellationRefundAsync(appointment);
            }

            await _appointmentRepo.DeleteAsync(id);
            await _appointmentRepo.SaveChangesAsync();
        }

        private PaginatedResult<AppointmentResponseDto> MapPaginatedResult(PaginatedResult<Appointment> result)
        {
            return new PaginatedResult<AppointmentResponseDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
        }

        private static AppointmentResponseDto MapToDto(Appointment a)
        {
            return new AppointmentResponseDto
            {
                Id = a.Id,
                Date = a.Date,
                TimeSlot = a.TimeSlot,
                PrescriptionId = a.Prescription != null ? a.Prescription.Id : null,
                Status = a.Status,
                Patient = new PatientMiniDto
                {
                    Id = a.Patient.Id,
                    FirstName = a.Patient.FirstName,
                    LastName = a.Patient.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = a.Doctor.Id,
                    FirstName = a.Doctor.FirstName,
                    LastName = a.Doctor.LastName
                },
                CreatedAt = a.CreatedAt,
            };
        }
    }
}