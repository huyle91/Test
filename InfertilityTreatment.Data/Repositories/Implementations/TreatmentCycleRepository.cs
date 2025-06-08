using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class TreatmentCycleRepository : BaseRepository<TreatmentCycle>, ITreatmentCycleRepository
    {
        public TreatmentCycleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TreatmentCycle>> GetByCustomerIdAsync(int customerId)
        {
            return await _dbSet
                .Where(tc => tc.CustomerId == customerId && tc.IsActive)
                .Include(tc => tc.Customer)
                .Include(tc => tc.Doctor)
                .Include(tc => tc.TreatmentPackage)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCycle>> GetByDoctorIdAsync(int doctorId)
        {
            return await _dbSet
                .Where(tc => tc.DoctorId == doctorId && tc.IsActive)
                .Include(tc => tc.Customer)
                .Include(tc => tc.Doctor)
                .Include(tc => tc.TreatmentPackage)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<TreatmentCycle>> GetByStatusAsync(CycleStatus status)
        {
            return await _dbSet
                .Where(tc => tc.Status == status && tc.IsActive)
                .Include(tc => tc.Customer)
                .Include(tc => tc.Doctor)
                .Include(tc => tc.TreatmentPackage)
                .OrderByDescending(tc => tc.CreatedAt)
                .ToListAsync();
        }

        public async Task<TreatmentCycle?> GetWithDetailsAsync(int cycleId)
        {
            return await _dbSet
                .Where(tc => tc.Id == cycleId && tc.IsActive)
                .Include(tc => tc.Customer)
                    .ThenInclude(c => c.User)
                .Include(tc => tc.Doctor)
                    .ThenInclude(d => d.User)
                .Include(tc => tc.TreatmentPackage)
                    .ThenInclude(tp => tp.TreatmentService)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetNextCycleNumberAsync(int customerId)
        {
            var maxCycleNumber = await _dbSet
                .Where(tc => tc.CustomerId == customerId && tc.IsActive)
                .MaxAsync(tc => (int?)tc.CycleNumber) ?? 0;
            
            return maxCycleNumber + 1;
        }
    }
}
