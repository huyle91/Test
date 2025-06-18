using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.DoctorSchedules;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class DoctorScheduleRepository : IDoctorScheduleRepository
    {
        private readonly ApplicationDbContext _context;
        public DoctorScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorSchedule?> GetByIdAsync(int id)
        {
            return await _context.DoctorSchedules.FindAsync(id);
        }


        public async Task<PaginatedResultDto<DoctorSchedule>> GetByDoctorIdAsync(int doctorId, PaginationQueryDTO pagination)
        {
            var query = _context.DoctorSchedules.Where(ds => ds.DoctorId == doctorId);
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pagination.PageNumber - 1) * pagination.PageSize).Take(pagination.PageSize).ToListAsync();
            return new PaginatedResultDto<DoctorSchedule>(
                items,
                totalCount,
                pagination.PageNumber,
                pagination.PageSize
                );
        }

        public async Task<DoctorSchedule> CreateAsync(DoctorSchedule dto)
        {
            var entity = new DoctorSchedule
            {
                DoctorId = dto.DoctorId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime
            };
            _context.DoctorSchedules.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> UpdateAsync(DoctorSchedule dto)
        {
            //var entity = await _context.DoctorSchedules.FindAsync(dto.Id);
            //if (entity == null) return false;
            //entity.DoctorId = dto.DoctorId;
            //entity.StartTime = dto.StartTime;
            //entity.EndTime = dto.EndTime;
            _context.DoctorSchedules.Update(dto);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _context.DoctorSchedules.FindAsync(id);
            if (entity == null) return false;
            _context.DoctorSchedules.Remove(entity);
            return await _context.SaveChangesAsync() > 0;
        }

        private DoctorScheduleDto MapToDto(DoctorSchedule entity)
        {
            return new DoctorScheduleDto
            {
                Id = entity.GetType().GetProperty("Id") != null ? (int)entity.GetType().GetProperty("Id").GetValue(entity) : 0,
                DoctorId = entity.DoctorId,
                StartTime = entity.StartTime,
                EndTime = entity.EndTime
            };
        }

        public async Task<List<DoctorSchedule>> GetSchedulesByDoctorAndDateAsync(int doctorId, DateTime date)
        {
            // Assuming schedules are recurring, filter by day of week
            // If schedules are for specific dates, add a Date property to DoctorSchedule and filter by that
            return await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<List<DoctorSchedule>> GetDoctorSchedulesByDateAsync(int doctorId, DateTime date)
        {
            // If DoctorSchedule is not date-specific, just return all for the doctor
            return await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId)
                .ToListAsync();
        }
    }
}
