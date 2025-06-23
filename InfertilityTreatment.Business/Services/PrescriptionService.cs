using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Prescription;
using InfertilityTreatment.Entity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class PrescriptionService : IPrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PrescriptionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //public async Task<PrescriptionDetailDto> CreatePrescriptionAsync(CreatePrescriptionDto dto)
        //{
        //    var prescription = _mapper.Map<Prescription>(dto);
        //    prescription.CreatedAt = DateTime.UtcNow;
        //    prescription.IsActive = true;

        //    await _unitOfWork.Prescriptions.AddAsync(prescription);
        //    await _unitOfWork.SaveChangesAsync();

        //    return _mapper.Map<PrescriptionDetailDto>(prescription);
        //}

        // Add more methods...
    }
}
