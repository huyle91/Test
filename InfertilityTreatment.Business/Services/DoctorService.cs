using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Doctors;
using InfertilityTreatment.Entity.Entities;
using Microsoft.Extensions.Logging;
using InfertilityTreatment.Business.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper, ILogger<DoctorService> logger)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResultDto<DoctorResponseDto>> GetAllDoctorsAsync(DoctorFilterDto filter)
        {
            var (doctors, totalCount) = await _doctorRepository.GetDoctorsAsync(filter);
            var doctorDtos = _mapper.Map<List<DoctorResponseDto>>(doctors);
            return new PaginatedResultDto<DoctorResponseDto>(doctorDtos, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<DoctorDetailDto> GetDoctorByIdAsync(int doctorId)
        {
            try
            {
                var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                if (doctor == null)
                    throw new NotFoundException($"Doctor with ID {doctorId} not found");

                return _mapper.Map<DoctorDetailDto>(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor by ID: {DoctorId}", doctorId);
                throw;
            }
        }

        public async Task<DoctorDetailDto> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                return null;
            }
            _mapper.Map(updateDoctorDto, doctor);
            if (doctor.User != null)
            {
                if (!string.IsNullOrEmpty(updateDoctorDto.FullName))
                    doctor.User.FullName = updateDoctorDto.FullName;
                if (!string.IsNullOrEmpty(updateDoctorDto.Email))
                    doctor.User.Email = updateDoctorDto.Email;
                if (!string.IsNullOrEmpty(updateDoctorDto.PhoneNumber))
                    doctor.User.PhoneNumber = updateDoctorDto.PhoneNumber;
            }
            await _doctorRepository.UpdateDoctorAsync(doctor);
            //var updated = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            return _mapper.Map<DoctorDetailDto>(doctor);
        }

        public async Task<DoctorDetailDto> UpdateAvailabilityAsync(int doctorId, bool isAvailable)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor != null)
            {
                doctor.IsAvailable = isAvailable;
                await _doctorRepository.UpdateDoctorAsync(doctor);
                //var updated = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                return _mapper.Map<DoctorDetailDto>(doctor);
            }
            return null;
        }

        public async Task<List<DoctorResponseDto>> SearchDoctorsAsync(DoctorSearchDto searchDto)
        {
            var doctors = await _doctorRepository.SearchDoctorsAsync(searchDto);
            return _mapper.Map<List<DoctorResponseDto>>(doctors);
        }

        public async Task<DoctorDetailDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto)
        {
            var doctor = _mapper.Map<Doctor>(createDoctorDto);
            await _doctorRepository.AddDoctorAsync(doctor);
            var created = await _doctorRepository.GetDoctorByIdAsync(doctor.Id);
            return _mapper.Map<DoctorDetailDto>(created);
        }

        public async Task<DoctorDetailDto> ToggleAvailabilityAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor != null)
            {

                return await UpdateAvailabilityAsync(doctorId, !doctor.IsAvailable);
                //doctor.IsAvailable = !doctor.IsAvailable;
                //await _doctorRepository.UpdateDoctorAsync(doctor);
                ////var updated = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                //return _mapper.Map<DoctorDetailDto>(doctor);
            }
            return null;
        }
    }
}
