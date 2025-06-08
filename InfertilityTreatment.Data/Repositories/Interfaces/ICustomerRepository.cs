using InfertilityTreatment.Entity.Entities;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<Customer?> GetWithUserAsync(int customerId);
        Task<Customer?> GetByUserIdAsync(int userId);
        Task<Customer?> GetWithMedicalHistoryAsync(int customerId);
    }
}