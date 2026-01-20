using ClinicCare.Business.Services.Interfaces;
using System.Data;
using ClinicCare.DataAccess.Models;
using ClinicCare.DataAccess.Repositories.Interfaces;
using ClinicCare.Shared.DTOs.Payment;

namespace ClinicCare.Business.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IGenericRepository<Payment> _repo;

        public PaymentService(IGenericRepository<Payment> repo)
        {
            _repo = repo;
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
            if (payment is null) return null;

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
            var payments = await _repo.FindAsync(p => p.SenderId == senderId);

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