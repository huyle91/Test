using InfertilityTreatment.Data.Context;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfertilityTreatment.Data.Repositories.Implementations
{
    public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(rt => rt.Token == token && rt.IsActive);
        }

        public async Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId)
        {
            return await _dbSet
                .Where(rt => rt.UserId == userId && rt.IsActive && rt.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task RevokeTokenAsync(string token)
        {
            var refreshToken = await GetByTokenAsync(token);
            if (refreshToken != null)
            {
                refreshToken.IsActive = false;
                refreshToken.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(refreshToken);
            }
        }

        public async Task RevokeAllUserTokensAsync(int userId)
        {
            var activeTokens = await GetActiveTokensByUserIdAsync(userId);
            foreach (var token in activeTokens)
            {
                token.IsActive = false;
                token.UpdatedAt = DateTime.UtcNow;
                await UpdateAsync(token);
            }
        }
    }
}
