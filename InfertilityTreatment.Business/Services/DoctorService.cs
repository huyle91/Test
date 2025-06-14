using AutoMapper;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Doctors;
using InfertilityTreatment.Entity.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResultDto<DoctorResponseDto>> GetAllDoctorsAsync(DoctorFilterDto filter)
        {
            var (doctors, totalCount) = await _doctorRepository.GetDoctorsAsync(filter);
            var doctorDtos = _mapper.Map<List<DoctorResponseDto>>(doctors);
            return new PaginatedResultDto<DoctorResponseDto>(doctorDtos, totalCount, filter.PageNumber, filter.PageSize);
        }

        public async Task<DoctorDetailDto> GetDoctorByIdAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            return _mapper.Map<DoctorDetailDto>(doctor);
        }

        public async Task<DoctorDetailDto> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorDto updateDoctorDto)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                return null;
            }
            _mapper.Map(updateDoctorDto, doctor);
            // Update User info if provided
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
            var updated = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            return _mapper.Map<DoctorDetailDto>(updated);
        }

        public async Task<DoctorDetailDto> UpdateAvailabilityAsync(int doctorId, bool isAvailable)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor != null)
            {
                doctor.IsAvailable = isAvailable;
                await _doctorRepository.UpdateDoctorAsync(doctor);
                var updated = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                return _mapper.Map<DoctorDetailDto>(updated);
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
            // Fetch with User included for response
            var created = await _doctorRepository.GetDoctorByIdAsync(doctor.Id);
            return _mapper.Map<DoctorDetailDto>(created);
        }

        public async Task<DoctorDetailDto> ToggleAvailabilityAsync(int doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor != null)
            {
                doctor.IsAvailable = !doctor.IsAvailable;
                await _doctorRepository.UpdateDoctorAsync(doctor);
                var updated = await _doctorRepository.GetDoctorByIdAsync(doctorId);
                return _mapper.Map<DoctorDetailDto>(updated);
            }
            return null;
        }
    }
}
