using ClinicCare.Business.Exceptions;
using ClinicCare.Business.Interfaces;
using ClinicCare.Business.Services.Interfaces;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Payment;
using ClinicCare.Shared.Enums;
using System.Data;

namespace ClinicCare.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _repo;
        private readonly IGenericRepository<Employee> _employeeRepo;
        private readonly IGenericRepository<Patient> _patientRepo;
        private readonly ICurrentUser _currentUser;

        public PaymentService(IGenericRepository<Payment> repo,
            IGenericRepository<Employee> employeeRepo,
            IGenericRepository<Patient> patientRepo,
            ICurrentUser currentUser)
        {
            _repo = repo;
            _currentUser = currentUser;
            _employeeRepo = employeeRepo;
            _patientRepo = patientRepo;
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetAllAsync()
        {
            var payments = await _repo.GetAllAsync();

            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                Amount = p.Amount,
                RecipientId = p.RecipientId,
                SenderId = p.SenderId,
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
                RecipientId = payment.RecipientId,
                SenderId = payment.SenderId,
            };
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetByRecipientAsync(Guid recipientId)
        {
            var recipient = _employeeRepo.GetByIdAsync(recipientId);
            if (recipient is null)
                throw new BadRequestException($"Recipient with Id{recipientId} not available");

            if (_currentUser.Role == UserRole.Doctor && _currentUser.UserId != recipientId)
                throw new ForbiddenException("You are not authorized");

            var payments = await _repo.FindAsync(p => p.RecipientId == recipientId);

            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                Amount = p.Amount,
                RecipientId = p.RecipientId,
                SenderId = p.SenderId,
            });
        }

        public async Task<IEnumerable<PaymentResponseDto>> GetBySenderAsync(Guid senderId)
        {
            var sender = _patientRepo.GetByIdAsync(senderId);
            if (sender is null)
                throw new BadRequestException($"Sender with Id{senderId} not available");

            if (_currentUser.Role == UserRole.Patient && _currentUser.UserId != senderId)
                throw new ForbiddenException("You are not authorized");

            IEnumerable<Payment> payments= [];

            if (_currentUser.Role == UserRole.Patient)
                payments = await _repo.FindAsync(p => p.SenderId == senderId);
            else if (_currentUser.Role == UserRole.Doctor)
                payments = await _repo.FindAsync(p => p.SenderId == senderId && p.RecipientId == _currentUser.UserId);

            return payments.Select(p => new PaymentResponseDto
            {
                Id = p.Id,
                Amount = p.Amount,
                RecipientId = p.RecipientId,
                SenderId = p.SenderId,
            });
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