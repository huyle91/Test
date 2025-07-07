using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IPaymentLogRepository : IBaseRepository<PaymentLog>
    {
        Task<List<PaymentLog>> GetPaymentLogsAsync(int paymentId);
        Task<List<PaymentLog>> GetLogsByActionAsync(string action);
    }
}
