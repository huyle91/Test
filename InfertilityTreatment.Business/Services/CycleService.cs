using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Implementations;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.TreatmentCycles;
using InfertilityTreatment.Entity.DTOs.TreatmentPhase;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class CycleService : ICycleService
    {
        private readonly ITreatmentCycleRepository _treatmentCycleRepository;
        private readonly ITreatmentPhaseRepository _treatmentPhaseRepository;
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        public CycleService(ITreatmentCycleRepository treatmentCycleRepository, IMapper mapper,ITreatmentPhaseRepository treatmentPhaseRepository,IDoctorRepository doctorRepository)
        {
            _treatmentCycleRepository = treatmentCycleRepository;
            _mapper = mapper;
            _treatmentPhaseRepository = treatmentPhaseRepository;
            _doctorRepository = doctorRepository;
        }
        public Task<PhaseResponseDto> AddPhaseAsync(int cycleId, CreatePhaseDto createPhaseDto)
        {
          var phase = _mapper.Map<TreatmentPhase>(createPhaseDto);
            phase.CycleId = cycleId;
            return _treatmentPhaseRepository.AddTreatmentPhaseAsync(phase)
                .ContinueWith(t => _mapper.Map<PhaseResponseDto>(t.Result));
        }
        public async Task<bool> AssignDoctorToCycleAsync(int cycleId, int doctorId)
        {
            try
            {
                // Validate doctor exists and is active
                var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                if (doctor == null || !doctor.IsActive)
                    return false;

                var cycle = await _treatmentCycleRepository.GetByIdAsync(cycleId);
                if (cycle == null || cycle.Status == CycleStatus.Completed)
                    return false;

                return await _treatmentCycleRepository.UpdateDoctorAsync(cycleId, doctorId);
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public Task<decimal> CalculateCycleCostAsync(int cycleId)
        {
           return _treatmentCycleRepository.CalculatePhaseCostAsync(cycleId);
        }
        public async Task<CycleResponseDto> CreateCycleAsync(CreateCycleDto createCycleDto)
        {
            try
            {
                var cycle = await _treatmentCycleRepository.AddTreatmentCycleAsync(_mapper.Map<TreatmentCycle>(createCycleDto));

                if (cycle == null)
                {
                    throw new Exception("Unable to create TreatmentCycle.");
                }

                return _mapper.Map<CycleResponseDto>(cycle);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the treatment cycle.", ex);
            }
        }
        public async Task<CycleDetailDto> GetCycleByIdAsync(int cycleId)
        {
            try
            {
                var cycle = await _treatmentCycleRepository.GetCycleByIdAsync(cycleId);

                if (cycle == null)
                {
                    throw new Exception("Unable to get TreatmentCycle.");
                }

                return _mapper.Map<CycleDetailDto>(cycle);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while creating the treatment cycle.", ex);
            }
        }
        public async Task<PaginatedResultDto<PhaseResponseDto>> GetCyclePhasesAsync(int cycleId, TreatmentPhaseFilterDto filter)
        {
            var pagedResult = await _treatmentPhaseRepository.GetCyclePhasesByCycleId(cycleId, filter);

            var mapped = _mapper.Map<List<PhaseResponseDto>>(pagedResult.Items);

            return new PaginatedResultDto<PhaseResponseDto>(
                mapped,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
        }

        public async Task<bool> UpdatePhaseAsync(int phaseId, UpdatePhaseDto updatePhaseDto)
        {
            var updated = await _treatmentPhaseRepository.UpdatePhaseAsync(
                phaseId,
                _mapper.Map<TreatmentPhase>(updatePhaseDto)
            );

            return updated;
        }
        public async Task<PaginatedResultDto<CycleResponseDto>> GetCyclesByCustomerAsync(int customerId, TreatmentCycleFilterDto filter)
        {
            var paginatedResult = await _treatmentCycleRepository.GetCyclesByCustomerAsync(customerId, filter);

            var mappedItems = paginatedResult.Items
                .Select(c => _mapper.Map<CycleResponseDto>(c))
                .ToList();

            return new PaginatedResultDto<CycleResponseDto>(
                mappedItems,
                paginatedResult.TotalCount,
                paginatedResult.PageNumber,
                paginatedResult.PageSize
            );
        }

        public async Task<PaginatedResultDto<CycleResponseDto>> GetCyclesByDoctorAsync(int doctorId, TreatmentCycleFilterDto filter)
        {
            var paginatedResult = await _treatmentCycleRepository.GetCyclesByDoctorAsync(doctorId, filter);

            var mappedItems = paginatedResult.Items
                .Select(c => _mapper.Map<CycleResponseDto>(c))
                .ToList();

            return new PaginatedResultDto<CycleResponseDto>(
                mappedItems,
                paginatedResult.TotalCount,
                paginatedResult.PageNumber,
                paginatedResult.PageSize
            );
        }

        Task<bool> ICycleService.UpdateCycleStatusAsync(int cycleId, CycleStatus status)
        {
            return _treatmentCycleRepository.UpdateStatusAsync(cycleId, status);
        }
        public Task<bool> UpdateCycleAsync(int cycleId, UpdateCycleDto dto)
        {
           var cycle = _mapper.Map<TreatmentCycle>(dto);
            cycle.Id = cycleId;
            return _treatmentCycleRepository.UpdateTreatmentCycleAsync(cycle);
        }
    }
}
