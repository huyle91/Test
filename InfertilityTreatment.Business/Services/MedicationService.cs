using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Medications;
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

        public MedicationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //public async Task<PaginatedResultDto<MedicationDetailDto>> GetMedicationsAsync(MedicationFilterDto filter)
        //{
        //    var medications = await _unitOfWork.Medications.GetPagedAsync(
        //        filter.PageNumber,
        //        filter.PageSize,
        //        m => m.IsActive
        //    );

        //    return _mapper.Map<PaginatedResultDto<MedicationDetailDto>>(medications);
        //}

        public async Task<MedicationDetailDto> GetMedicationByIdAsync(int id)
        {
            var medication = await _unitOfWork.Medications.GetByIdAsync(id);
            return _mapper.Map<MedicationDetailDto>(medication);
        }

        // Add more CRUD methods...
    }

}
