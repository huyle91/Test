using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Constants;
using InfertilityTreatment.Entity.DTOs.TreatmentServices;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class TreatmentServiceService : ITreatmentServiceService
    {
        private readonly ITreatmentServiceRepository _repo;
        private readonly IMapper _mapper;

        public TreatmentServiceService(ITreatmentServiceRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<TreatmentServiceDto>> GetAllAsync() =>
            _mapper.Map<List<TreatmentServiceDto>>(await _repo.GetAllAsync());

        public async Task<TreatmentServiceDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TreatmentServiceDto>(entity);
        }

        public async Task<int> CreateAsync(CreateTreatmentServiceDto dto)
        {
            var isDuplicate = await _repo.IsNameExistsAsync(dto.Name);
            if (isDuplicate)
                throw new ArgumentException(TreatmentServiceMessages.NameHasAlreadyExist);

            var entity = _mapper.Map<TreatmentService>(dto);
            entity.CreatedAt = DateTime.UtcNow;
            await _repo.AddAsync(entity);
            return entity.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateTreatmentServiceDto dto)
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
