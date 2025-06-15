using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Doctors;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class DoctorRepository : BaseRepository<Doctor>, IDoctorRepository
    {
        public DoctorRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Doctor> Doctors, int TotalCount)> GetDoctorsAsync(DoctorFilterDto filter)
        {
            Expression<Func<Doctor, bool>> predicate = d =>
        (string.IsNullOrEmpty(filter.Specialization) || d.Specialization.Contains(filter.Specialization)) &&
        (!filter.IsAvailable.HasValue || d.IsAvailable == filter.IsAvailable.Value) &&
        (!filter.MinYearsOfExperience.HasValue || d.YearsOfExperience >= filter.MinYearsOfExperience.Value);

            var allDoctors = await FindWithIncludeAsync(predicate, d => d.User);

            var totalCount = allDoctors.Count();

            var pagedDoctors = allDoctors
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            return (pagedDoctors, totalCount);
        }

        public async Task<Doctor> GetDoctorByIdAsync(int doctorId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == doctorId);
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(int doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Doctor>> SearchDoctorsAsync(DoctorSearchDto searchDto)
        {
            Expression<Func<Doctor, bool>> predicate = d =>
                                        (string.IsNullOrEmpty(searchDto.Query) ||
                                        d.User.FullName.Contains(searchDto.Query) ||
                                        d.Specialization.Contains(searchDto.Query)) &&
                                        (string.IsNullOrEmpty(searchDto.Specialization) ||
                                        d.Specialization.Contains(searchDto.Specialization)) &&
                                        (!searchDto.IsAvailable.HasValue ||
                                        d.IsAvailable == searchDto.IsAvailable.Value);
            return await FindWithIncludeAsync(predicate, d => d.User);
        }
    }
}
