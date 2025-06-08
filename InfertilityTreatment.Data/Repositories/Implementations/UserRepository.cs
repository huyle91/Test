using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _dbSet
                .Include(u => u.Customer)
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<User?> GetByIdWithProfilesAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Customer)
                .Include(u => u.Doctor)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }
    }
}
