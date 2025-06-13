using InfertilityTreatment.Entity.DTOs.TreatmentServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface ITreatmentServiceService
    {
        Task<List<TreatmentServiceDto>> GetAllAsync();
        Task<TreatmentServiceDto?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateTreatmentServiceDto dto);
        Task<bool> UpdateAsync(int id, UpdateTreatmentServiceDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
