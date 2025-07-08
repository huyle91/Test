using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Medications;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IMedicationService
    {
        Task<PaginatedResultDto<MedicationDetailDto>> GetAllMedicationsAsync(MedicationFilterDto filters, PaginationQueryDTO pagination);
        Task<MedicationDetailDto?> GetMedicationByIdAsync(int id);
        Task<MedicationDetailDto> CreateMedicationAsync(CreateMedicationDto dto);
        Task<MedicationDetailDto> UpdateMedicationAsync(int id, UpdateMedicationDto dto);
        Task<bool> DeleteMedicationAsync(int id);
        Task<PaginatedResultDto<MedicationDetailDto>> SearchMedicationsAsync(string query, PaginationQueryDTO pagination);
    }
}
