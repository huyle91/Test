using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetWithUserAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive);
        }

        public async Task<Customer?> GetByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsActive);
        }

        public async Task<Customer?> GetWithMedicalHistoryAsync(int customerId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.TreatmentCycles)
                .FirstOrDefaultAsync(c => c.Id == customerId && c.IsActive);
        }
    }
}
