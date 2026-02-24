using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Pagination;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepo;
        private readonly ICurrentUser _currentUser;

        public PaymentService(
            IPaymentRepository paymentRepo,
            ICurrentUser currentUser)
        {
            _paymentRepo = paymentRepo;
            _currentUser = currentUser;
        }

        public async Task<PaginatedResult<PaymentResponseDto>> GetAllAsync(PaymentSearchParams searchParams)
        {
            PaginatedResult<Payment> result = _currentUser.Role switch
            {
                UserRole.Admin => await _paymentRepo.GetAllAsync(searchParams),
                UserRole.Doctor => await _paymentRepo.GetAllAsync(searchParams, patientId: null, doctorId: _currentUser.UserId),
                UserRole.Patient => await _paymentRepo.GetAllAsync(searchParams, patientId: _currentUser.UserId, doctorId: null),
                _ => throw new ForbiddenException("Invalid role")
            };

            return MapPaginatedResult(result);
        }

        public async Task<PaymentResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var payment = await _paymentRepo.GetByIdAsync(id);
            if (payment is null)
                throw new NotFoundException($"Payment with id {id} not found.");

            if(_currentUser.Role == UserRole.Patient && _currentUser.UserId != payment.PatientId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != payment.DoctorId)
                throw new ForbiddenException("You are not authorized");

            return MapToDto(payment);
        }
        
        public async Task<Guid> CreateAsync(PaymentCreateDto dto, PaymentType paymentType = PaymentType.Paid)
        {
            ValidationHelper.NotNull(dto, "Payment data is required.");

            if(paymentType == PaymentType.Paid)
            {
                if (dto.PatientId != _currentUser.UserId)
                    throw new ForbiddenException("Sender does not match logged-in user.");
            }

            if (dto.PatientId == dto.DoctorId)
                throw new ValidationException("Sender and recipient cannot be the same.");

            if (dto.Amount <= 0)
                throw new ValidationException("Payment amount must be greater than zero.");

            if (dto.Amount > 10000000)
                throw new ValidationException("Payment amount exceeds allowed limit.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                TransactionId = PaymentHelper.GenerateTransactionId(),
                Amount = dto.Amount,
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                Type = paymentType,
            };

            await _paymentRepo.InsertAsync(payment);
            await _paymentRepo.SaveChangesAsync();

            return payment.Id;
        }

        public async Task ProcessCancellationRefundAsync(Appointment appointment)
        {
            var payment = await _paymentRepo.GetByIdAsync(appointment.PaymentId);

            if (payment is null)
                throw new BadRequestException($"No payment found for cancelled appointment {appointment.Id}");

            var paymentCreatedDto = new PaymentCreateDto
            {
                Amount = payment.Amount,
                PatientId = payment.PatientId,
                DoctorId = payment.DoctorId,
            };

            await CreateAsync(paymentCreatedDto, PaymentType.Refund);
            await _paymentRepo.SaveChangesAsync();
        }

        private PaginatedResult<PaymentResponseDto> MapPaginatedResult(PaginatedResult<Payment> result)
        {
            return new PaginatedResult<PaymentResponseDto>
            {
                Items = result.Items.Select(MapToDto).ToList(),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalPages = result.TotalPages,
                HasPreviousPage = result.HasPreviousPage,
                HasNextPage = result.HasNextPage
            };
        }

        private static PaymentResponseDto MapToDto(Payment p)
        {
            return new PaymentResponseDto
            {
                Id = p.Id,
                TransactionId = p.TransactionId,
                Amount = p.Amount,
                Patient = new PatientMiniDto
                {
                    Id = p.PatientId,
                    FirstName = p.Patient.FirstName,
                    LastName = p.Patient.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = p.DoctorId,
                    FirstName = p.Doctor.FirstName,
                    LastName = p.Doctor.LastName
                },
                Type = p.Type,
                CreatedAt = p.CreatedAt,
            };
        }
    }
}