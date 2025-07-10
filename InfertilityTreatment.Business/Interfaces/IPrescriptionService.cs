using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Prescription;
using InfertilityTreatment.Entity.DTOs.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IPrescriptionService
    {
        Task<PrescriptionDetailDto> CreatePrescriptionAsync(int phaseId, CreatePrescriptionDto dto);
        Task<PaginatedResultDto<PrescriptionDetailDto>> GetPrescriptionsByPhaseAsync(int phaseId, PaginationQueryDTO pagination);
        Task<PrescriptionDetailDto?> GetPrescriptionByIdAsync(int id);
        Task<PrescriptionDetailDto> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto dto);
        Task<bool> DeletePrescriptionAsync(int id);
        Task<PaginatedResultDto<PrescriptionSummaryDto>> GetPrescriptionsByCustomerAsync(int customerId, PaginationQueryDTO pagination);
        Task<PaginatedResultDto<PrescriptionSummaryDto>> GetActivePrescriptionsAsync(int customerId, PaginationQueryDTO pagination);
        Task<PaginatedResultDto<PrescriptionSummaryDto>> GetAllPrescriptionsAsync(PaginationQueryDTO pagination);
    }
}
