using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface ITreatmentServiceRepository
    {
        Task<List<TreatmentService>> GetAllAsync();
        Task<TreatmentService?> GetByIdAsync(int id);
        Task AddAsync(TreatmentService entity);
        Task<bool> UpdateAsync(TreatmentService entity);
        Task<bool> DeleteAsync(int id);

        Task<bool> IsNameExistsAsync(string name);
    }
}
