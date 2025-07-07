using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class PaymentLogRepository : BaseRepository<PaymentLog>, IPaymentLogRepository
    {
        public PaymentLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<PaymentLog>> GetPaymentLogsAsync(int paymentId)
        {
            return await _context.PaymentLogs
                .Include(pl => pl.Payment)
                .Where(pl => pl.PaymentId == paymentId)
                .OrderByDescending(pl => pl.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<PaymentLog>> GetLogsByActionAsync(string action)
        {
            return await _context.PaymentLogs
                .Include(pl => pl.Payment)
                .Where(pl => pl.Action == action)
                .OrderByDescending(pl => pl.CreatedAt)
                .ToListAsync();
        }
    }
}
