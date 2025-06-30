using AutoMapper;
using InfertilityTreatment.Business.Exceptions;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.TreatmentPhase;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class TreatmentPhaseService : ITreatmentPhaseService
    {
        private readonly ITreatmentPhaseRepository _treatmentPhaseRepository;
        private readonly IMapper _mapper;

        public TreatmentPhaseService(ITreatmentPhaseRepository treatmentPhaseRepository, IMapper mapper)
        {
            _treatmentPhaseRepository = treatmentPhaseRepository;
            _mapper = mapper;
        }

        public async Task<PhaseResponseDto> CreatePhaseAsync(CreatePhaseDto dto)
        {
            var treatmentPhase = _mapper.Map<TreatmentPhase>(dto);
            treatmentPhase.CreatedAt = DateTime.UtcNow;
            treatmentPhase.IsActive = true;

            var createdPhase = await _treatmentPhaseRepository.AddTreatmentPhaseAsync(treatmentPhase);
            return _mapper.Map<PhaseResponseDto>(createdPhase);
        }

        public async Task<List<PhaseResponseDto>> GetPhasesByCycleAsync(int cycleId)
        {
            var filter = new TreatmentPhaseFilterDto
            {
                PageNumber = 1,
                PageSize = int.MaxValue // Get all phases for the cycle
            };

            var result = await _treatmentPhaseRepository.GetCyclePhasesByCycleId(cycleId, filter);
            return _mapper.Map<List<PhaseResponseDto>>(result.Items);
        }

        public async Task<PhaseResponseDto> GetPhaseByIdAsync(int phaseId)
        {
            var phase = await _treatmentPhaseRepository.GetByIdAsync(phaseId);
            if (phase == null)
            {
                throw new NotFoundException($"Treatment phase with ID {phaseId} not found.");
            }

            return _mapper.Map<PhaseResponseDto>(phase);
        }

        public async Task<PhaseResponseDto> UpdatePhaseAsync(int phaseId, UpdatePhaseDto dto)
        {
            var existingPhase = await _treatmentPhaseRepository.GetByIdAsync(phaseId);
            if (existingPhase == null)
            {
                throw new NotFoundException($"Treatment phase with ID {phaseId} not found.");
            }

            _mapper.Map(dto, existingPhase);
            existingPhase.UpdatedAt = DateTime.UtcNow;

            var success = await _treatmentPhaseRepository.UpdatePhaseAsync(phaseId, existingPhase);
            if (!success)
            {
                throw new InvalidOperationException("Failed to update treatment phase.");
            }

            return _mapper.Map<PhaseResponseDto>(existingPhase);
        }

        public async Task<bool> DeletePhaseAsync(int phaseId)
        {
            var existingPhase = await _treatmentPhaseRepository.GetByIdAsync(phaseId);
            if (existingPhase == null)
            {
                throw new NotFoundException($"Treatment phase with ID {phaseId} not found.");
            }

            return await _treatmentPhaseRepository.DeleteAsync(phaseId);
        }
    }
}
