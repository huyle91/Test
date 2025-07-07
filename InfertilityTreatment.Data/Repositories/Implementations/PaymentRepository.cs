using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByPaymentIdAsync(string paymentId)
        {
            return await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.TreatmentPackage)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<PaginatedResultDto<Payment>> GetPaymentHistoryAsync(int customerId, int page, int pageSize)
        {
            var query = _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.TreatmentPackage)
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt);

            var totalCount = await query.CountAsync();
            var payments = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResultDto<Payment>(payments, totalCount, page, pageSize);
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Include(p => p.Customer)
                .Where(p => p.Status == Entity.Enums.PaymentStatus.Pending)
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetCompletedPaymentsAsync(DateTime fromDate, DateTime toDate)
        {
            return await _context.Payments
                .Include(p => p.Customer)
                .Include(p => p.TreatmentPackage)
                .Where(p => p.Status == Entity.Enums.PaymentStatus.Completed &&
                           p.ProcessedAt >= fromDate &&
                           p.ProcessedAt <= toDate)
                .OrderByDescending(p => p.ProcessedAt)
                .ToListAsync();
        }
    }
}
