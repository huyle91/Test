using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public  interface ITreatmentPackageService
    {
        Task<List<TreatmentPackageDto>> GetAllAsync();
        Task<TreatmentPackageDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateTreatmentPackageDto dto);
        Task<bool> UpdateAsync(int id, UpdateTreatmentPackageDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
