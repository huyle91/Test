using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
   public interface ITreatmentPackageRepository
    {
        Task<List<TreatmentPackage>> GetAllAsync();
        Task<TreatmentPackage?> GetByIdAsync(int id);
        Task AddAsync(TreatmentPackage entity);
        Task<bool> UpdateAsync(TreatmentPackage entity);
        Task<bool> DeleteAsync(int id);

        Task<bool> IsPackageNameExistsAsync(string packageName);
    }
}
