using AutoMapper;
using InfertilityTreatment.Business.Exceptions;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
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
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var treatmentPhase = _mapper.Map<TreatmentPhase>(dto);
            treatmentPhase.CreatedAt = DateTime.UtcNow;
            treatmentPhase.IsActive = true;

            var createdPhase = await _treatmentPhaseRepository.AddTreatmentPhaseAsync(treatmentPhase);
            return _mapper.Map<PhaseResponseDto>(createdPhase);
        }

        public async Task<List<PhaseResponseDto>> GetPhasesByCycleAsync(int cycleId)
        {
            if (cycleId <= 0)
                throw new ArgumentException("Cycle ID must be positive", nameof(cycleId));

            var filter = new TreatmentPhaseFilterDto
            {
                PageNumber = 1,
                PageSize = 1000 // Reasonable limit instead of int.MaxValue to prevent memory issues
            };

            var result = await _treatmentPhaseRepository.GetCyclePhasesByCycleId(cycleId, filter);
            return _mapper.Map<List<PhaseResponseDto>>(result.Items);
        }

        public async Task<PaginatedResultDto<PhaseResponseDto>> GetPhasesByCycleAsync(int cycleId, TreatmentPhaseFilterDto filter)
        {
            if (cycleId <= 0)
                throw new ArgumentException("Cycle ID must be positive", nameof(cycleId));

            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            var result = await _treatmentPhaseRepository.GetCyclePhasesByCycleId(cycleId, filter);
            var mappedItems = _mapper.Map<List<PhaseResponseDto>>(result.Items);
            
            return new PaginatedResultDto<PhaseResponseDto>(
                mappedItems,
                result.TotalCount,
                result.PageNumber,
                result.PageSize
            );
        }

        public async Task<PhaseResponseDto> GetPhaseByIdAsync(int phaseId)
        {
            if (phaseId <= 0)
                throw new ArgumentException("Phase ID must be positive", nameof(phaseId));

            var phase = await _treatmentPhaseRepository.GetByIdAsync(phaseId);
            if (phase == null)
            {
                throw new NotFoundException($"Treatment phase with ID {phaseId} not found.");
            }

            return _mapper.Map<PhaseResponseDto>(phase);
        }

        public async Task<PhaseResponseDto> UpdatePhaseAsync(int phaseId, UpdatePhaseDto dto)
        {
            if (phaseId <= 0)
                throw new ArgumentException("Phase ID must be positive", nameof(phaseId));
            
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

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
            if (phaseId <= 0)
                throw new ArgumentException("Phase ID must be positive", nameof(phaseId));

            var existingPhase = await _treatmentPhaseRepository.GetByIdAsync(phaseId);
            if (existingPhase == null)
            {
                throw new NotFoundException($"Treatment phase with ID {phaseId} not found.");
            }

            return await _treatmentPhaseRepository.DeleteAsync(phaseId);
        }
    }
}
