using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.TreatmentPakages;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class TreatmentPackageService : ITreatmentPackageService
    {
        private readonly ITreatmentPackageRepository _repo;
        private readonly IMapper _mapper;

        public TreatmentPackageService(ITreatmentPackageRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<TreatmentPackageDto>> GetAllAsync() =>
            _mapper.Map<List<TreatmentPackageDto>>(await _repo.GetAllAsync());

        public async Task<TreatmentPackageDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TreatmentPackageDto>(entity);
        }

        public async Task<int> CreateAsync(CreateTreatmentPackageDto dto)
        {
            var isDuplicate = await _repo.IsPackageNameExistsAsync(dto.PackageName);
            if (isDuplicate)
                throw new ArgumentException(TreatmentPackageMessages.NameHasAlreadyExist);

            var entity = _mapper.Map<TreatmentPackage>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _repo.AddAsync(entity);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateTreatmentPackageDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            _mapper.Map(dto, entity);
            entity.UpdatedAt = DateTime.UtcNow;
            return await _repo.UpdateAsync(entity);
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);
    }
}
