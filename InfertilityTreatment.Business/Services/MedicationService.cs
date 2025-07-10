using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Medications;
using InfertilityTreatment.Entity.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class MedicationService : IMedicationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MedicationService> _logger;

        public MedicationService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MedicationService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResultDto<MedicationDetailDto>> GetAllMedicationsAsync(MedicationFilterDto filters, PaginationQueryDTO pagination)
        {
            // Get all medications matching the filter criteria
            var allMedications = await _unitOfWork.Medications.FindAsync(m => 
                (filters.IsActive == null || m.IsActive == filters.IsActive) &&
                (string.IsNullOrEmpty(filters.Name) || m.Name.Contains(filters.Name)) &&
                (string.IsNullOrEmpty(filters.ActiveIngredient) || m.ActiveIngredient.Contains(filters.ActiveIngredient)) &&
                (string.IsNullOrEmpty(filters.Manufacturer) || m.Manufacturer.Contains(filters.Manufacturer)));

            var totalCount = allMedications.Count();

            var sortedMedications = filters.SortDirection?.ToLower() == "desc" 
                ? allMedications.OrderByDescending(GetSortExpression(filters.SortBy).Compile())
                : allMedications.OrderBy(GetSortExpression(filters.SortBy).Compile());

            var pagedMedications = sortedMedications
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var medicationDtos = _mapper.Map<List<MedicationDetailDto>>(pagedMedications);

            return new PaginatedResultDto<MedicationDetailDto>(medicationDtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        public async Task<MedicationDetailDto?> GetMedicationByIdAsync(int id)
        {
            var medication = await _unitOfWork.Medications.GetByIdAsync(id);
            if (medication == null)
                return null;
            
            return _mapper.Map<MedicationDetailDto>(medication);
        }

        public async Task<MedicationDetailDto> CreateMedicationAsync(CreateMedicationDto dto)
        {
            // Check if medication with same name already exists
            var existingMedication = await _unitOfWork.Medications.FindAsync(m => m.Name.ToLower() == dto.Name.ToLower());
            if (existingMedication.Any())
            {
                throw new ArgumentException($"Medication with name '{dto.Name}' already exists.");
            }

            var medication = _mapper.Map<Medication>(dto);
            medication.CreatedAt = DateTime.UtcNow;
            medication.IsActive = true;

            await _unitOfWork.Medications.AddAsync(medication);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Created medication: {medication.Name} (ID: {medication.Id})");
            return _mapper.Map<MedicationDetailDto>(medication);
        }

        public async Task<MedicationDetailDto> UpdateMedicationAsync(int id, UpdateMedicationDto dto)
        {
            var medication = await _unitOfWork.Medications.GetByIdAsync(id);
            if (medication == null)
                throw new ArgumentException("Medication not found");

            // Check if another medication with same name already exists (excluding current medication)
            var existingMedication = await _unitOfWork.Medications.FindAsync(m => 
                m.Name.ToLower() == dto.Name.ToLower() && m.Id != id);
            if (existingMedication.Any())
            {
                throw new ArgumentException($"Another medication with name '{dto.Name}' already exists.");
            }

            _mapper.Map(dto, medication);
            medication.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Medications.UpdateAsync(medication);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Updated medication: {medication.Name} (ID: {medication.Id})");
            return _mapper.Map<MedicationDetailDto>(medication);
        }

        public async Task<bool> DeleteMedicationAsync(int id)
        {
            var medication = await _unitOfWork.Medications.GetByIdAsync(id);
            if (medication == null)
                return false;

            // Soft delete
            medication.IsActive = false;
            medication.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Medications.UpdateAsync(medication);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Soft deleted medication: {medication.Name} (ID: {medication.Id})");
            return true;
        }

        public async Task<PaginatedResultDto<MedicationDetailDto>> SearchMedicationsAsync(string query, PaginationQueryDTO pagination)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new PaginatedResultDto<MedicationDetailDto>(new List<MedicationDetailDto>(), 0, pagination.PageNumber, pagination.PageSize);

            var allMedications = await _unitOfWork.Medications.FindAsync(m => 
                m.IsActive && 
                (m.Name.Contains(query) || 
                 m.ActiveIngredient.Contains(query) || 
                 m.Manufacturer.Contains(query)));

            var totalCount = allMedications.Count();

            var sortedMedications = allMedications.OrderBy(m => m.Name);

            var pagedMedications = sortedMedications
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            var medicationDtos = _mapper.Map<List<MedicationDetailDto>>(pagedMedications);

            return new PaginatedResultDto<MedicationDetailDto>(medicationDtos, totalCount, pagination.PageNumber, pagination.PageSize);
        }

        private static System.Linq.Expressions.Expression<Func<Medication, object>> GetSortExpression(string? sortBy)
        {
            return sortBy?.ToLower() switch
            {
                "name" => m => m.Name,
                "activeingredient" => m => m.ActiveIngredient,
                "manufacturer" => m => m.Manufacturer,
                "createdat" => m => m.CreatedAt,
                _ => m => m.Name
            };
        }
    }
}
