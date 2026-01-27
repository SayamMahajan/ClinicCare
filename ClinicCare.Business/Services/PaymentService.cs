using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Helpers;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Employee;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _repo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ICurrentUser _currentUser;

        public PaymentService(
            IGenericRepository<Payment> repo,
            IPaymentRepository paymenttRepo,
            ICurrentUser currentUser)
        {
            _repo = repo;
            _paymentRepo = paymenttRepo;
            _currentUser = currentUser;
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllAsync()
        {
            IEnumerable<Payment> payments = _currentUser.Role switch
            {
                UserRole.Admin => await _repo.GetAllAsync(),
                UserRole.Doctor => await _paymentRepo.GetPaymentsForDoctorAsync(_currentUser.UserId),
                UserRole.Patient => await _paymentRepo.GetPaymentsForPatientAsync(_currentUser.UserId),
                _ => throw new ForbiddenException("Invalid role")
            };

            return payments.Select(MapToDto);
        }
           
        public async Task<PaymentResponseDto?> GetByIdAsync(Guid id)
        {
            ValidationHelper.GuidNotEmpty(id, nameof(id));

            var payment = await _repo.GetByIdAsync(id);
            if (payment is null)
                throw new NotFoundException($"Payment with id {id} not found.");

            if(_currentUser.Role == UserRole.Patient && _currentUser.UserId != payment.SenderId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != payment.RecipientId)
                throw new ForbiddenException("You are not authorized");

            return MapToDto(payment);
        }
        
        public async Task<Guid> CreateAsync(PaymentCreateDto dto)
        {
            ValidationHelper.NotNull(dto, "Payment data is required.");

            if (dto.SenderId != _currentUser.UserId)
                throw new ForbiddenException("Sender does not match logged-in user.");

            if (dto.SenderId == dto.RecipientId)
                throw new ValidationException("Sender and recipient cannot be the same.");

            if (dto.Amount <= 0)
                throw new ValidationException("Payment amount must be greater than zero.");

            if (dto.Amount > 10000000)
                throw new ValidationException("Payment amount exceeds allowed limit.");

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                Amount = dto.Amount,
                RecipientId = dto.RecipientId,
                SenderId = dto.SenderId,
            };

            await _repo.InsertAsync(payment);
            await _repo.SaveChangesAsync();

            return payment.Id;
        }

        private static PaymentResponseDto MapToDto(Payment p)
        {
            return new PaymentResponseDto
            {
                Id = p.Id,
                Amount = p.Amount,
                Patient = new PatientMiniDto
                {
                    Id = p.Sender.Id,
                    FirstName = p.Sender.FirstName,
                    LastName = p.Sender.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = p.Recipient.Id,
                    FirstName = p.Recipient.FirstName,
                    LastName = p.Recipient.LastName
                },
                CreatedAt = p.CreatedAt,
            };
        }
    }
}