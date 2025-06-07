using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<Customer?> GetWithMedicalHistoryAsync(int customerId);
        Task<Customer?> GetWithTreatmentCyclesAsync(int customerId);
        Task<Customer?> GetByUserIdAsync(int userId);
    }
}