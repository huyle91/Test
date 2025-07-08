using InfertilityTreatment.Entity.DTOs.Doctors;
using InfertilityTreatment.Entity.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfertilityTreatment.Entity.DTOs.Common; 

namespace InfertilityTreatment.Data.Repositories.Interfaces
{
    public interface IDoctorRepository
    {
        Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetDoctorsAsync(DoctorFilterDto filter);
        Task<Doctor> GetDoctorByIdAsync(int doctorId);
        Task<Doctor?> GetByUserIdAsync(int userId);
        Task AddDoctorAsync(Doctor doctor);
        Task UpdateDoctorAsync(Doctor doctor);
        Task<bool> UpdateSuccessRateAsync(int doctorId,decimal? successRate);
        Task DeleteDoctorAsync(int doctorId);
        Task<IEnumerable<Doctor>> SearchDoctorsAsync(DoctorSearchDto searchDto);
    }
}
