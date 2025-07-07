using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Doctors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Interfaces
{
    public interface IDoctorService
    {
        Task<PaginatedResultDto<DoctorResponseDto>> GetAllDoctorsAsync(DoctorFilterDto filter);
        Task<DoctorDetailDto> GetDoctorByIdAsync(int doctorId);
        Task<DoctorDetailDto> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorDto updateDoctorDto);
        Task<DoctorDetailDto> UpdateAvailabilityAsync(int doctorId, bool isAvailable);
        Task<List<DoctorResponseDto>> SearchDoctorsAsync(DoctorSearchDto searchDto);
        Task<DoctorDetailDto> CreateDoctorAsync(CreateDoctorDto createDoctorDto);
        Task<DoctorDetailDto> ToggleAvailabilityAsync(int doctorId);       

    }
}
