using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface ITreatmentCycleRepository : IBaseRepository<TreatmentCycle>
    {
        Task<IEnumerable<TreatmentCycle>> GetByCustomerIdAsync(int customerId);
        Task<IEnumerable<TreatmentCycle>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<TreatmentCycle>> GetByStatusAsync(CycleStatus status);
        Task<TreatmentCycle?> GetWithDetailsAsync(int cycleId);
        Task<int> GetNextCycleNumberAsync(int customerId);
    }
}