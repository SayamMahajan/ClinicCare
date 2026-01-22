using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Doctor;
using ClinicCare.Shared.DTOs.Patient;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
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

            return payments.Select(p => new PaymentResponseDto
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
                }
            });
        }
           
        public async Task<PaymentResponseDto?> GetByIdAsync(Guid id)
        {
            var payment = await _repo.GetByIdAsync(id);
            if (payment is null)
                throw new NotFoundException($"Payment with id {id} not found.");

            if(_currentUser.Role == UserRole.Patient && _currentUser.UserId != payment.SenderId)
                throw new ForbiddenException("You are not authorized");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != payment.RecipientId)
                throw new ForbiddenException("You are not authorized");

            return new PaymentResponseDto
            {
                Id = payment.Id,
                Amount = payment.Amount,
                Patient = new PatientMiniDto
                {
                    Id = payment.Sender.Id,
                    FirstName = payment.Sender.FirstName,
                    LastName = payment.Sender.LastName
                },
                Doctor = new DoctorMiniDto
                {
                    Id = payment.Recipient.Id,
                    FirstName = payment.Recipient.FirstName,
                    LastName = payment.Recipient.LastName
                }
            };
        }
        
        public async Task<Guid> CreateAsync(PaymentCreateDto dto)
        {
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
    }
}