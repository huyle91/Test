using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Prescription;
using InfertilityTreatment.Entity.DTOs.Prescriptions;
using InfertilityTreatment.Entity.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PrescriptionService> _logger;

        public PrescriptionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PrescriptionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PrescriptionDetailDto> CreatePrescriptionAsync(int phaseId, CreatePrescriptionDto dto)
        {
            // Validate phase exists
            var phase = await _unitOfWork.TreatmentPhases.GetByIdAsync(phaseId);
            if (phase == null)
                throw new ArgumentException("Treatment phase not found");

            // Validate medication exists
            var medication = await _unitOfWork.Medications.GetByIdAsync(dto.MedicationId);
            if (medication == null)
                throw new ArgumentException("Medication not found");

            // Validate start date < end date
            if (dto.StartDate >= dto.EndDate)
                throw new ArgumentException("Start date must be before end date");

            // Validate không cho trùng medicationId nếu cùng phase
            var existingPrescription = await _unitOfWork.Prescriptions.FindAsync(p => 
                p.PhaseId == phaseId && 
                p.MedicationId == dto.MedicationId && 
                p.IsActive);
            if (existingPrescription.Any())
            {
                throw new ArgumentException($"A prescription for medication '{medication.Name}' already exists in this treatment phase");
            }

            var prescription = _mapper.Map<Prescription>(dto);
            prescription.PhaseId = phaseId;
            prescription.CreatedAt = DateTime.UtcNow;
            prescription.IsActive = true;

            await _unitOfWork.Prescriptions.AddAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Created prescription for phase {phaseId} with medication {dto.MedicationId}");
            
            var result = await GetPrescriptionByIdAsync(prescription.Id);
            return result!;
        }

        public async Task<PrescriptionDetailDto?> GetPrescriptionByIdAsync(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
            if (prescription == null)
                return null;

            // Load related entities manually
            prescription.Medication = await _unitOfWork.Medications.GetByIdAsync(prescription.MedicationId);
            prescription.TreatmentPhase = await _unitOfWork.TreatmentPhases.GetByIdAsync(prescription.PhaseId);

            return _mapper.Map<PrescriptionDetailDto>(prescription);
        }

        public async Task<PrescriptionDetailDto> UpdatePrescriptionAsync(int id, UpdatePrescriptionDto dto)
        {
            var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
            if (prescription == null)
                throw new ArgumentException("Prescription not found");

            _mapper.Map(dto, prescription);
            prescription.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Prescriptions.UpdateAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Updated prescription {id}");
            
            var result = await GetPrescriptionByIdAsync(id);
            return result!;
        }

        public async Task<bool> DeletePrescriptionAsync(int id)
        {
            var prescription = await _unitOfWork.Prescriptions.GetByIdAsync(id);
            if (prescription == null)
                return false;

            // Soft delete
            prescription.IsActive = false;
            prescription.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Prescriptions.UpdateAsync(prescription);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Soft deleted prescription {id}");
            return true;
        }

        public async Task<PaginatedResultDto<PrescriptionSummaryDto>> GetPrescriptionsByCustomerAsync(int customerId, PaginationQueryDTO pagination)
        {
            // Get all treatment cycles for the customer
            var cycles = await _unitOfWork.TreatmentCycles.FindAsync(c => c.CustomerId == customerId);
            
            // Get all phases for these cycles
            var allPhases = new List<TreatmentPhase>();
            foreach (var cycle in cycles)
            {
                var phases = await _unitOfWork.TreatmentPhases.GetPhasesByCycleIdAsync(cycle.Id);
                allPhases.AddRange(phases);
            }
            
            var phaseIds = allPhases.Select(p => p.Id).ToList();
            
            var allPrescriptions = await _unitOfWork.Prescriptions.FindAsync(
                p => phaseIds.Contains(p.PhaseId) && p.IsActive);

            //// Apply search filter if provided
            //if (!string.IsNullOrEmpty(pagination.SearchTerm))
            //{
            //    var searchTerm = pagination.SearchTerm.ToLower();
            //    allPrescriptions = allPrescriptions.Where(p => 
            //        p.Medication != null && p.Medication.Name.ToLower().Contains(searchTerm) ||
            //        p.Instructions.ToLower().Contains(searchTerm));
            //}

            var totalCount = allPrescriptions.Count();

            var sortedPrescriptions = allPrescriptions.OrderBy(p => p.StartDate);

            var pagedPrescriptions = sortedPrescriptions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            // Load related entities manually
            foreach (var prescription in pagedPrescriptions)
            {
                prescription.Medication = await _unitOfWork.Medications.GetByIdAsync(prescription.MedicationId);
                prescription.TreatmentPhase = allPhases.FirstOrDefault(p => p.Id == prescription.PhaseId);
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionSummaryDto>>(pagedPrescriptions);

            return new PaginatedResultDto<PrescriptionSummaryDto>(prescriptionDtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PaginatedResultDto<PrescriptionSummaryDto>> GetActivePrescriptionsAsync(int customerId, PaginationQueryDTO pagination)
        {
            var currentDate = DateTime.UtcNow;
            
            // Get all treatment cycles for the customer
            var cycles = await _unitOfWork.TreatmentCycles.FindAsync(c => c.CustomerId == customerId);
            
            // Get all phases for these cycles
            var allPhases = new List<TreatmentPhase>();
            foreach (var cycle in cycles)
            {
                var phases = await _unitOfWork.TreatmentPhases.GetPhasesByCycleIdAsync(cycle.Id);
                allPhases.AddRange(phases);
            }
            
            var phaseIds = allPhases.Select(p => p.Id).ToList();
            
            var allActivePrescriptions = await _unitOfWork.Prescriptions.FindAsync(
                p => phaseIds.Contains(p.PhaseId) && 
                     p.IsActive && 
                     p.StartDate <= currentDate && 
                     p.EndDate >= currentDate);

            //// Apply search filter if provided
            //if (!string.IsNullOrEmpty(pagination.SearchTerm))
            //{
            //    var searchTerm = pagination.SearchTerm.ToLower();
            //    allActivePrescriptions = allActivePrescriptions.Where(p => 
            //        p.Medication != null && p.Medication.Name.ToLower().Contains(searchTerm) ||
            //        p.Instructions.ToLower().Contains(searchTerm));
            //}

            var totalCount = allActivePrescriptions.Count();

            var sortedPrescriptions = allActivePrescriptions.OrderBy(p => p.StartDate);

            var pagedPrescriptions = sortedPrescriptions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            // Load related entities manually
            foreach (var prescription in pagedPrescriptions)
            {
                prescription.Medication = await _unitOfWork.Medications.GetByIdAsync(prescription.MedicationId);
                prescription.TreatmentPhase = allPhases.FirstOrDefault(p => p.Id == prescription.PhaseId);
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionSummaryDto>>(pagedPrescriptions);

            return new PaginatedResultDto<PrescriptionSummaryDto>(prescriptionDtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PaginatedResultDto<PrescriptionSummaryDto>> GetAllPrescriptionsAsync(PaginationQueryDTO pagination)
        {
            var allPrescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.IsActive);

            //// Apply search filter if provided
            //if (!string.IsNullOrEmpty(pagination.SearchTerm))
            //{
            //    var searchTerm = pagination.SearchTerm.ToLower();
            //    allPrescriptions = allPrescriptions.Where(p => 
            //        p.Medication != null && p.Medication.Name.ToLower().Contains(searchTerm) ||
            //        p.Instructions.ToLower().Contains(searchTerm));
            //}

            var totalCount = allPrescriptions.Count();

            var sortedPrescriptions = allPrescriptions.OrderBy(p => p.StartDate);

            var pagedPrescriptions = sortedPrescriptions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            // Load related entities manually
            foreach (var prescription in pagedPrescriptions)
            {
                prescription.Medication = await _unitOfWork.Medications.GetByIdAsync(prescription.MedicationId);
                prescription.TreatmentPhase = await _unitOfWork.TreatmentPhases.GetByIdAsync(prescription.PhaseId);
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionSummaryDto>>(pagedPrescriptions);

            return new PaginatedResultDto<PrescriptionSummaryDto>(prescriptionDtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<PaginatedResultDto<PrescriptionDetailDto>> GetPrescriptionsByPhaseAsync(int phaseId, PaginationQueryDTO pagination)
        {
            // Validate phase exists
            var phase = await _unitOfWork.TreatmentPhases.GetByIdAsync(phaseId);
            if (phase == null)
                throw new ArgumentException("Treatment phase not found");

            var allPrescriptions = await _unitOfWork.Prescriptions.FindAsync(p => p.PhaseId == phaseId && p.IsActive);

            //// Apply search filter if provided
            //if (!string.IsNullOrEmpty(pagination.SearchTerm))
            //{
            //    var searchTerm = pagination.SearchTerm.ToLower();
            //    allPrescriptions = allPrescriptions.Where(p => 
            //        p.Medication != null && p.Medication.Name.ToLower().Contains(searchTerm) ||
            //        p.Instructions.ToLower().Contains(searchTerm));
            //}

            var totalCount = allPrescriptions.Count();

            var sortedPrescriptions = allPrescriptions.OrderBy(p => p.StartDate);

            var pagedPrescriptions = sortedPrescriptions
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            // Load related entities manually
            foreach (var prescription in pagedPrescriptions)
            {
                prescription.Medication = await _unitOfWork.Medications.GetByIdAsync(prescription.MedicationId);
                prescription.TreatmentPhase = phase;
            }

            var prescriptionDtos = _mapper.Map<List<PrescriptionDetailDto>>(pagedPrescriptions);

            return new PaginatedResultDto<PrescriptionDetailDto>(prescriptionDtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }
    }
}
