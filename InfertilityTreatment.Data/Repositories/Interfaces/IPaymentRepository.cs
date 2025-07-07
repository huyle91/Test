using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<Payment?> GetByPaymentIdAsync(string paymentId);
        Task<PaginatedResultDto<Payment>> GetPaymentHistoryAsync(int customerId, int page, int pageSize);
        Task<List<Payment>> GetPendingPaymentsAsync();
        Task<List<Payment>> GetCompletedPaymentsAsync(DateTime fromDate, DateTime toDate);
    }
}
