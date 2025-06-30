using InfertilityTreatment.Entity.DTOs.TreatmentPhase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface ITreatmentPhaseService
    {
        Task<PhaseResponseDto> CreatePhaseAsync(CreatePhaseDto dto);
        Task<List<PhaseResponseDto>> GetPhasesByCycleAsync(int cycleId);
        Task<PhaseResponseDto> GetPhaseByIdAsync(int phaseId);
        Task<PhaseResponseDto> UpdatePhaseAsync(int phaseId, UpdatePhaseDto dto);
        Task<bool> DeletePhaseAsync(int phaseId);
    }
}
